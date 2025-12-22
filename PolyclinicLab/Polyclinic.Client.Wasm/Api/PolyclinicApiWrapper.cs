namespace Polyclinic.Client.Wasm.Api;

/// <summary>
/// Advanced wrapper for accessing Polyclinic API with enhanced error handling, 
/// collection management, and complex operations following teacher's best practices.
/// </summary>
public class PolyclinicApiWrapper(HttpClient httpClient, IConfiguration configuration)
{
    private readonly PolyclinicClient _client = new(
        configuration["Api:Url"] ?? httpClient.BaseAddress?.ToString() ?? "https://localhost:7048", 
        httpClient);

    public async Task<IList<PatientDto>> GetAllPatients() => [.. await _client.GetPatientsAsync()];
    public async Task<PatientDto> GetPatient(int id) => await _client.GetPatientAsync(id);
    public async Task<PatientDto> CreatePatient(CreateUpdatePatientDto dto) => await _client.CreatePatientAsync(dto);
    public async Task<PatientDto> UpdatePatient(int id, CreateUpdatePatientDto dto) => await _client.UpdatePatientAsync(id, dto);
    public async Task DeletePatient(int id) => await _client.DeletePatientAsync(id);
    
    public async Task<IList<DoctorDto>> GetAllDoctors() => [.. await _client.GetDoctorsAsync()];
    public async Task<DoctorDto> GetDoctor(int id) => await _client.GetDoctorAsync(id);
    public async Task<DoctorDto> CreateDoctor(CreateUpdateDoctorDto dto) => await _client.CreateDoctorAsync(dto);
    public async Task<DoctorDto> UpdateDoctor(int id, CreateUpdateDoctorDto dto) => await _client.UpdateDoctorAsync(id, dto);
    public async Task DeleteDoctor(int id) => await _client.DeleteDoctorAsync(id);

    public async Task<IList<AppointmentDto>> GetAllAppointments() => [.. await _client.GetAppointmentsAsync()];
    public async Task<AppointmentDto> GetAppointment(int id) => await _client.GetAppointmentAsync(id);
    public async Task<AppointmentDto> CreateAppointment(CreateUpdateAppointmentDto dto) => await _client.CreateAppointmentAsync(dto);
    public async Task<AppointmentDto> UpdateAppointment(int id, CreateUpdateAppointmentDto dto) => await _client.UpdateAppointmentAsync(id, dto);
    public async Task DeleteAppointment(int id) => await _client.DeleteAppointmentAsync(id);

    public async Task<IList<DoctorDto>> GetExperiencedDoctors(int minExperience = 10) => [.. await _client.GetExperiencedDoctorsAsync(minExperience)];
    public async Task<IList<PatientDto>> GetPatientsByDoctor(int doctorId) => [.. await _client.GetPatientsByDoctorAsync(doctorId)];
    public async Task<RepeatedAppointmentsAnalyticsDto> GetRepeatedAppointments(int months = 1) => await _client.GetRepeatedAppointmentsAsync(months);
    public async Task<IList<PatientAnalyticsDto>> GetPatientsOlderThanWithMultipleDoctors(int age = 30) => [.. await _client.GetPatientsOlderThanWithMultipleDoctorsAsync(age)];
    public async Task<IList<AppointmentDto>> GetAppointmentsInRoom(int room = 101) => [.. await _client.GetAppointmentsInRoomAsync(room)];
}