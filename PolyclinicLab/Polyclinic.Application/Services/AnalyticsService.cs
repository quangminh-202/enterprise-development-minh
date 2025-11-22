using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;

namespace Polyclinic.Application.Services;

public class AnalyticsService(
    IRepository<Doctor, int> doctorRepo,
    IRepository<Appointment, int> appointmentRepo,
    IMapper mapper
) : IAnalyticsService
{
    // (1) Doctors with >=10 years of experience
    public List<DoctorDto> GetExperiencedDoctors(int minExperience)
    {
        var doctors = doctorRepo.ReadAll()
            .Where(d => d.Experience >= minExperience)
            .ToList();
        return mapper.Map<List<DoctorDto>>(doctors);
    }

    public bool DoctorExists(int doctorId)
    {
        return doctorRepo.ReadAll().Any(d => d.Id == doctorId);
    }

    // (2) Patients who visited a specific doctor, ordered by name
    public List<PatientDto> GetPatientsByDoctor(int doctorId)
    {
        var patients = appointmentRepo.ReadAll()
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p!.FullName);
        return mapper.Map<List<PatientDto>>(patients);
    }

    // (3) Count of repeated appointments in the N months
    public RepeatedAppointmentsAnalyticsDto GetRepeatedAppointments(int months)
    {
        var now = DateTime.Now;
        var fromDate = now.AddMonths(-months);
        var repeatedAppointments = appointmentRepo.ReadAll()
            .Where(a => a.IsRepeated && a.Date >= fromDate && a.Date <= now)
            .ToList();
        var dtoList = mapper.Map<List<AppointmentDto>>(repeatedAppointments);
        return new RepeatedAppointmentsAnalyticsDto(
            dtoList.Count,
            dtoList
        );
    }

    // (4) Patients >X years old with appointments with >1 distinct doctor
    public List<PatientAnalyticsDto> GetPatientsOlderThanWithMultipleDoctors(int age)
    {
        var today = DateTime.Today;
        var cutoffDate = today.AddYears(-age);
        
        return appointmentRepo.ReadAll()
            .Where(a => a.Patient != null && a.Doctor != null)
            .GroupBy(a => a.Patient!.Id)
            .Where(g =>
            {
                var patient = g.First().Patient;
                return patient?.BirthDate <= cutoffDate &&
                       g.Select(a => a.Doctor!.Id).Distinct().Count() > 1;
            })
            .OrderBy(g => g.First().Patient!.BirthDate)
            .Select(g =>
            {
                var patient = g.First().Patient!;
                return new PatientAnalyticsDto(
                    patient.Id,
                    patient.FullName,
                    GetAge(patient.BirthDate, today),
                    g.Select(a => a.Doctor!.Id).Distinct().Count()
                );
            })
            .ToList();
    }

    // Accurate age calculation considering birthday/month
    private static int GetAge(DateTime birthday, DateTime now)
    {
        var year = now.Year - birthday.Year;
        return birthday.AddYears(year) <= now ? year : year - 1;
    }

    // (5) Appointments in a specific room within current month
    public List<AppointmentDto> GetAppointmentsInRoom(int room)
    {
        var now = DateTime.Now;
        var firstDay = new DateTime(now.Year, now.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);
        var appointments = appointmentRepo.ReadAll()
            .Where(a => a.Room == room && a.Date >= firstDay && a.Date <= lastDay)
            .ToList();
        return mapper.Map<List<AppointmentDto>>(appointments);
    }
}
