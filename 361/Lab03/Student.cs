using System.ComponentModel.DataAnnotations;

namespace Lab02.Models;

public enum Specialization
{
    Mathematics,
    ComputerScience,
    Physics
}
public class Student
{
    [Required(ErrorMessage = "Id must be provided", 
        ErrorMessageResourceName = nameof(Id))]
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(3, ErrorMessageResourceName = nameof(Name),
        ErrorMessage = $"{nameof(Name)} must be at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10)]
    public double Average { get; set; }
    public Specialization Specialization { get; set; }
}