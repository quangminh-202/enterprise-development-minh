namespace Polyclinic.Application.Contracts;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data Transfer Object (DTO) used for creating and updating patients.
/// This DTO does not include the Id field as it's auto-generated.
/// </summary>
public class CreateUpdatePatientDto
{
    [Required(ErrorMessage = "Passport is required")]
    public string Passport { get; set; } = "";

    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = "Male";

    [Required(ErrorMessage = "Birth date is required")]
    public DateTime BirthDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Blood type is required")]
    public string BloodType { get; set; } = "A";

    [Required(ErrorMessage = "Rh factor is required")]
    public string RhFactor { get; set; } = "Positive";

    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = "";
}