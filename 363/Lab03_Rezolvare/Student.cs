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
    [Range(1, int.MaxValue, ErrorMessage = "Id not in range")]
    public int Id { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Name must contain at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 10, ErrorMessage = "Average must be between 1 and 10")]
    public double Average { get; set; }
    public Specialization Specialization { get; set; }
}
