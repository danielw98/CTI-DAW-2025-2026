using Lab03_Rezolvare;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_Rezolvare.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private static readonly List<Student> students = new()
    {
        new Student { Id = 1, Name = "Ana", Average = 9.10, Specialization =
        Specialization.ComputerScience },
        new Student { Id = 2, Name = "Mihai", Average = 7.50, Specialization =
        Specialization.Mathematics },
        new Student { Id = 3, Name = "Ioana", Average = 8.30, Specialization =
        Specialization.Physics },
        new Student { Id = 4, Name = "Andrei", Average = 6.90, Specialization =
        Specialization.ComputerScience },
        new Student { Id = 5, Name = "Maria", Average = 9.60, Specialization =
        Specialization.Mathematics },
        new Student { Id = 6, Name = "Alex", Average = 4.7, Specialization =
        Specialization.Physics }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(students);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Student? found = students.FirstOrDefault(student => student.Id == id);
        if (found == null)
        {
            return NotFound();
        }
        return Ok(found);
    }

    [HttpPost]
    public IActionResult Create(CreateStudentRequest? student)
    {
        if (student == null)
        {
            return BadRequest("Missing body.");
        }
        int newId = students.Max(student => student.Id) + 1;

        Student newStud = new();
        newStud.Id = newId;
        newStud.Name = student.Name.Trim();
        newStud.Specialization = student.Specialization;
        newStud.Average = student.Average;
        students.Add(newStud);
        return CreatedAtAction(nameof(GetById), new { id = newStud.Id }, newStud);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Student? student = students.FirstOrDefault(student => student.Id == id);
        if (student == null)
        {
            return NotFound();
        }
        students.Remove(student);
        return NoContent();
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] UpdateStudentRequest? student)
    {
        if (student == null)
        {
            return BadRequest("Student must not be null");
        }

        Student? updatedStudent = students.FirstOrDefault(s => s.Id == student.Id);
        if (updatedStudent == null)
        {
            return NotFound($"Student with id {student.Id} doesn't exist.");
        }

        updatedStudent.Name = student.Name;
        updatedStudent.Average = student.Average;
        updatedStudent.Specialization = student.Specialization;
        return Ok(updatedStudent);
    }

    [HttpGet("filter")]
    public IActionResult Filter([FromQuery] double? minAverage)
    {
        if (minAverage == null)
        {
            return BadRequest("Must not be null.");
        }


        List<Student> filteredStudents = students.Where(s => s.Average > minAverage).ToList();

        return Ok(filteredStudents);
    }

    [HttpGet("top")]
    public IActionResult Top([FromQuery] double? minAverage)
    {
        if (minAverage == null)
        {
            return BadRequest("Must not be null.");
        }

        List<Student> filteredStudents = students
            .Where(s => s.Average > minAverage)
            .OrderByDescending(students => students.Average)
            .ToList();

        return Ok(filteredStudents);
    }

    [HttpGet("stats")]

    public IActionResult Stats()
    {
        bool anyComputerScience = students.Any(s => s.Specialization == Specialization.ComputerScience);
        bool allPassing = students.All(s => s.Average >= 5);
        int numStudents = students.Count;
        double totalAverage = students.Average(s => s.Average);
        double maxAverage = students.Max(s => s.Average);
        double minAverage = students.Min(s => s.Average);

        return Ok(new { anyComputerScience, allPassing, numStudents, totalAverage, maxAverage, minAverage });
    }

    [HttpGet("specializations")]

    public IActionResult Specializations()
    {
        var specializations = students
            .Select(student => student.Specialization)
            .Distinct();

        return Ok(specializations);
    }

    [HttpGet("stats-by-specialization")]
    public IActionResult StatsBySpecialization()
    {
        var stats = students
            .GroupBy(s => s.Specialization)
            .Select(s => new {
                Count = s.Count(),
                Average = s.Average(s => s.Average),
                minAverage = s.Min(s => s.Average),
                maxAverage = s.Max(s => s.Average)
            });
        return Ok(stats);
    }

    [HttpGet("search")]
    public IActionResult Search(string text, double? minAverage)
    {
        var filteredStudents = students
            .Where(s => s.Name.ToLower().Contains(text.ToLower().Trim()));

        if (minAverage != null)
            filteredStudents = filteredStudents.Where(s => s.Average >= minAverage);

        return Ok(filteredStudents);
    }

    [HttpGet("page")]
    public IActionResult Pagination(int page = 1, int pageSize = 3)
    {
        int count = students.Count;
        if(page < 1 || pageSize < 1 || page > count/pageSize)
        {
            return BadRequest();
        }
        var paginatedStudents = students.Skip((page - 1) * pageSize).Take(pageSize);

        return Ok(paginatedStudents);
    }
}
