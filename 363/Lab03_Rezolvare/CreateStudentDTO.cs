using System.ComponentModel.DataAnnotations;

namespace Lab03_Rezolvare;

public class CreateStudentDTO
{
    [Required]
    [MinLength(3, ErrorMessage = "Name must contain at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 10, ErrorMessage = "Average must be between 1 and 10")]
    public double Average { get; set; }
    public Specialization Specialization { get; set; }
}
