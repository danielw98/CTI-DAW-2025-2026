using System.ComponentModel.DataAnnotations;

namespace Lab03;

public class CreateStudentRequest
{
    [Required(AllowEmptyStrings = false)]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10)]
    public double Average { get; set; }
    public Specialization Specialization { get; set; }
}
