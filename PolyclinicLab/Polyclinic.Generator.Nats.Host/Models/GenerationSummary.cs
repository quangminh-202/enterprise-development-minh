namespace Polyclinic.Generator.Nats.Host.Models;

/// <summary>
/// Summary of generation results (optimized response without full data)
/// </summary>
public record GenerationSummary(
    int TotalGenerated,
    int PatientsGenerated,
    int DoctorsGenerated,
    int AppointmentsGenerated,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan Duration,
    List<Guid> BatchIds,
    int TotalBatches
);
