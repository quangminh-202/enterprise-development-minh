using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Enums;
using Polyclinic.Domain.Models;

namespace Polyclinic.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping between Patient domain model and DTOs
/// </summary>
public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        // Patient <-> PatientDto (bidirectional mapping)
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => src.BloodType.ToString()))
            .ForMember(dest => dest.RhFactor, opt => opt.MapFrom(src => src.RhFactor.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender)))
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => Enum.Parse<BloodType>(src.BloodType)))
            .ForMember(dest => dest.RhFactor, opt => opt.MapFrom(src => Enum.Parse<RhFactor>(src.RhFactor)))
            .ForMember(dest => dest.Passport, opt => opt.Ignore());

        // CreateUpdatePatientDto -> Patient
        CreateMap<CreateUpdatePatientDto, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Passport, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender)))
            .ForMember(dest => dest.BloodType, opt => opt.MapFrom(src => Enum.Parse<BloodType>(src.BloodType)))
            .ForMember(dest => dest.RhFactor, opt => opt.MapFrom(src => Enum.Parse<RhFactor>(src.RhFactor)));
    }
}
