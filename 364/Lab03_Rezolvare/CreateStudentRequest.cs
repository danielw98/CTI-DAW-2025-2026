using System.ComponentModel.DataAnnotations;

namespace Lab03_Rezolvare;

public class CreateStudentRequest
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;
    [Range(1, 10)]
    public double Average { get; set; }
    [Required]
    public Specialization Specialization { get; set; }
}
