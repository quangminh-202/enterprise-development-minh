using Polyclinic.Application.Contracts;

namespace Polyclinic.Application.Interfaces;

/// <summary>
/// Interface for appointment-related business operations.
/// Extends the generic application service with appointment-specific functionality.
/// </summary>
public interface IAppointmentService : IApplicationService<AppointmentDto, CreateUpdateAppointmentDto, int>
{}
