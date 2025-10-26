using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;

namespace Polyclinic.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping between Doctor domain model and DTOs
/// </summary>
public class DoctorMappingProfile : Profile
{
    public DoctorMappingProfile()
    {
        // Doctor <-> DoctorDto (bidirectional mapping)
        CreateMap<Doctor, DoctorDto>().ReverseMap();

        // CreateUpdateDoctorDto -> Doctor
        CreateMap<CreateUpdateDoctorDto, Doctor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Passport, opt => opt.Ignore())
            .ForMember(dest => dest.BirthYear, opt => opt.Ignore());
    }
}
