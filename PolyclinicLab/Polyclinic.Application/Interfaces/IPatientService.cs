using Polyclinic.Application.Contracts;

namespace Polyclinic.Application.Interfaces;

/// <summary>
/// Interface for patient-related business operations.
/// Extends the generic application service with patient-specific functionality.
/// </summary>
public interface IPatientService : IApplicationService<PatientDto, CreateUpdatePatientDto, int>
{}





