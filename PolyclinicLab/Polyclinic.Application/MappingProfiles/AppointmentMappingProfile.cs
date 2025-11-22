using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;

namespace Polyclinic.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping between Appointment domain model and DTOs
/// </summary>
public class AppointmentMappingProfile : Profile
{   
    public AppointmentMappingProfile()
    {
        // Appointment -> AppointmentDto
        CreateMap<Appointment, AppointmentDto>()
            .ForCtorParam("DoctorName", opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty))
            .ForCtorParam("PatientName", opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty));

        // CreateUpdateAppointmentDto -> Appointment
        CreateMap<CreateUpdateAppointmentDto, Appointment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId));
    }
}
