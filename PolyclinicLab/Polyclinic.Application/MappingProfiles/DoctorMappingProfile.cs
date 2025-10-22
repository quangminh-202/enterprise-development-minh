using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;

namespace Polyclinic.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for mapping between Doctor domain model and DoctorDto
/// </summary>
public class DoctorMappingProfile : Profile
{
    public DoctorMappingProfile()
    {
        CreateMap<Doctor, DoctorDto>();
        CreateMap<DoctorDto, Doctor>();
    }
}
