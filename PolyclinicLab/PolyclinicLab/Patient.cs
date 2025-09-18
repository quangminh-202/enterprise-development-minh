namespace Polyclinic.Domain;
public enum Gender { Male, Female}
public enum BloodType { A, B, AB, O}
public enum RhFactor { Positive, Negative }

public class Patient
{
    public string Passport { get; set; }
    public string FullName { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public string Address { get; set; }
    public BloodType BloodType { get; set; }
    public RhFactor RhFactor { get; set; }
    public string Phone { get; set; }
}
