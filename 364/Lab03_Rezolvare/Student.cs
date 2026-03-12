using System.ComponentModel.DataAnnotations;

namespace Lab03_Rezolvare;

public enum Specialization
{
    Mathematics,
    ComputerScience,
    Physics
}
public class Student
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Id { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Name must be at least 3 chars long.")]
    public string Name { get; set; } = string.Empty;
    [Range(1, 10, ErrorMessage = "Average must be between 1 and 10")]
    public double Average { get; set; }
    [Required]
    public Specialization Specialization { get; set; }
}
