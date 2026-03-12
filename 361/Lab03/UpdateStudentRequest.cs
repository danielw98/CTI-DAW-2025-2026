using Lab02.Models;
using System.ComponentModel.DataAnnotations;

namespace Lab02;

public class UpdateStudentRequest
{
    [Required(AllowEmptyStrings = false)]
    [MinLength(3, ErrorMessageResourceName = nameof(Name),
    ErrorMessage = $"{nameof(Name)} must be at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10)]
    public double Average { get; set; }
    public Specialization Specialization { get; set; }
}
