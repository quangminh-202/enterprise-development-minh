using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;

namespace Polyclinic.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping between Patient domain model and PatientDto
/// </summary>
public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<Patient, PatientDto>();
        CreateMap<PatientDto, Patient>();
    }
}
