using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IRepository<Doctor, int> _doctorRepo;
    private readonly IRepository<Patient, int> _patientRepo;
    private readonly IRepository<Appointment, int> _appointmentRepo;
    private readonly IMapper _mapper;

    public AnalyticsService(
        IRepository<Doctor, int> doctorRepo,
        IRepository<Patient, int> patientRepo,
        IRepository<Appointment, int> appointmentRepo,
        IMapper mapper)
    {
        _doctorRepo = doctorRepo;
        _patientRepo = patientRepo;
        _appointmentRepo = appointmentRepo;
        _mapper = mapper;
    }

    // (1) Doctors with >=10 years of experience
    public List<DoctorDto> GetExperiencedDoctors(int minExperience)
    {
        var doctors = _doctorRepo.ReadAll()
            .Where(d => d.Experience >= minExperience)
            .ToList();

        return _mapper.Map<List<DoctorDto>>(doctors);
    }

    public bool DoctorExists(int doctorId)
    {
        return _doctorRepo.ReadAll().Any(d => d.Id == doctorId);
    }

    // (2) Patients who visited a specific doctor, ordered by name
    public List<PatientDto> GetPatientsByDoctor(int doctorId)
    {
        var patients = _appointmentRepo.ReadAll()
            .Where(a => a.Doctor.Id == doctorId)
            .Select(a => a.Patient)
            .Distinct()
            .OrderBy(p => p.FullName)
            .ToList();

        return _mapper.Map<List<PatientDto>>(patients);
    }

    // (3) Count of repeated appointments in the N months
    public RepeatedAppointmentsAnalyticsDto GetRepeatedAppointments(int months)
    {
        var now = DateTime.Now;
        var fromDate = now.AddMonths(-months);

        var repeatedAppointments = _appointmentRepo.ReadAll()
            .Where(a => a.IsRepeated && a.Date >= fromDate && a.Date <= now)
            .ToList();

        var dtoList = _mapper.Map<List<AppointmentDto>>(repeatedAppointments);

        return new RepeatedAppointmentsAnalyticsDto(
            dtoList.Count,
            dtoList
        );
    }

    // (4) Patients >30 years old with appointments with >1 distinct doctor
    public List<PatientAnalyticsDto> GetPatientsOlderThanWithMultipleDoctors(int age)
    {
        var today = DateTime.Today;
        var cutoffDate = today.AddYears(-age);

        var result = _appointmentRepo.ReadAll()
            .GroupBy(a => a.Patient)
            .Where(g =>
                g.Key.BirthDate <= cutoffDate &&
                g.Select(a => a.Doctor).Distinct().Count() > 1)
            .OrderBy(g => g.Key.BirthDate)
            .Select(g => new PatientAnalyticsDto(
                g.Key.Id,
                g.Key.FullName,
                today.Year - g.Key.BirthDate.Year,
                g.Select(a => a.Doctor).Distinct().Count()
            ))
            .ToList();

        return result;
    }

    // (5) Appointments in a specific room within current month
    public List<AppointmentDto> GetAppointmentsInRoom(int room)
    {
        var now = DateTime.Now;
        var firstDay = new DateTime(now.Year, now.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var appointments = _appointmentRepo.ReadAll()
            .Where(a => a.Room == room && a.Date >= firstDay && a.Date <= lastDay)
            .ToList();

        return _mapper.Map<List<AppointmentDto>>(appointments);
    }
}
