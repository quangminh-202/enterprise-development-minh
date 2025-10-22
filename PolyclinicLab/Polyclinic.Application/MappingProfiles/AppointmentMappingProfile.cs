using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;

namespace Polyclinic.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping between Appointment domain model and AppointmentDto
/// </summary>
public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        // Domain → DTO mapping
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName));
        
        // DTO → Domain mapping (simplified for basic operations)
        CreateMap<AppointmentDto, Appointment>()
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore());
    }
}
