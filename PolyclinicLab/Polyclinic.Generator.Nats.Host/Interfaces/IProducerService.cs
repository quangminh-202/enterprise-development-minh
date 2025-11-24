using Polyclinic.Application.Contracts;

namespace Polyclinic.Generator.Nats.Host.Interfaces;

/// <summary>
/// Service for sending contracts via NATS
/// </summary>
public interface IProducerService
{
    /// <summary>
    /// Sends a batch of patients
    /// </summary>
    Task<BatchAckResponse> SendPatientsAsync<T>(IList<T> patients);
    
    /// <summary>
    /// Sends a batch of doctors
    /// </summary>
    Task<BatchAckResponse> SendDoctorsAsync<T>(IList<T> doctors);
    
    /// <summary>
    /// Sends a batch of appointments
    /// </summary>
    Task<BatchAckResponse> SendAppointmentsAsync<T>(IList<T> appointments);
}
