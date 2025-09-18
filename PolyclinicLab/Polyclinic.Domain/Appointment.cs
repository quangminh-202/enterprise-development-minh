namespace Polyclinic.Domain;

public class Appointment
{
    public DateTime Date { get; set; }
    public string Room { get; set; }
    public bool IsRepeated { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
}
