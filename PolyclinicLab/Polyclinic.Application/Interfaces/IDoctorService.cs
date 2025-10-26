using Polyclinic.Application.Contracts;

namespace Polyclinic.Application.Interfaces;

/// <summary>
/// Interface for doctor-related business operations.
/// Extends the generic application service with doctor-specific functionality.
/// </summary>
public interface IDoctorService : IApplicationService<DoctorDto, CreateUpdateDoctorDto, int>
{}