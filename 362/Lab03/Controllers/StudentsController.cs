using Lab03;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private static readonly List<Student> _students = new()
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
        return Ok(_students);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive.");
        }
        Student? found = _students
            .Where(s => s.Id == id)
            .FirstOrDefault();

        if (found == null)
        {
            return NotFound();
        }
        return Ok(found);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateStudentRequest? student)
    {
        if (student == null)
        {
            return BadRequest("Missing body.");
        }
        if (string.IsNullOrWhiteSpace(student.Name) || student.Name.Trim().Length
       < 3)
        {
            return BadRequest("Name must be at least 3 characters long.");
        }
        Student newStudent = new()
        {
            Id = _students.Count > 0 ? _students.Max(s => s.Id) + 1 : 1,
            Name = student.Name.Trim(),
            Average = student.Average,
            Specialization = student.Specialization
        };
        _students.Add(newStudent);
        return CreatedAtAction(nameof(GetById), new { id = newStudent.Id }, newStudent);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive.");
        }

        var studentToRemove = _students.FirstOrDefault(s => s.Id == id);
        if (studentToRemove == null)
        {
            return NotFound();
        }
        _students.Remove(studentToRemove);

        return NoContent();
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] Student? student)
    {
        if (student == null)
        {
            return BadRequest("Missing body.");
        }
        if (student.Name.Trim().Length < 3)
        {
            return BadRequest("Name must be at least 3 characters long.");
        }
        

        var filterStudent = _students.FirstOrDefault(s => s.Id == student.Id);
        if (filterStudent == null)
        {
            return NotFound();
        }

        filterStudent.Name = student.Name;
        filterStudent.Specialization = student.Specialization;
        filterStudent.Average = student.Average;

        return Ok(filterStudent);
    }



    [HttpGet ("filter")]
    public IActionResult Filter (double? minAverage)
    {
        if (minAverage is null)
        {
            return BadRequest("Parameter minAverage must be specified");
        }

        if (minAverage < 1 || minAverage > 10)
        {
            return BadRequest("Parameter minAverage must be between 1 and 10");
        }

        var filteredStudents = new List<Student>();

        foreach (var student in _students)
        {
            if (student.Average >= minAverage)
            {
                filteredStudents.Add(student);
            }
        }

        return Ok(filteredStudents);
    }

    [HttpGet ("stats")]
    public IActionResult GetStats()
    {
        bool anyComputerScience = _students.Any(s => s.Specialization == Specialization.ComputerScience);
        bool allPassing = _students.All(s => s.Average >= 5);


        var stats = new
        {
            AnyComputerScience = anyComputerScience,
            TotiTrecuti = allPassing,
            NumarStudenti = _students.Count(),
            MediaGenerala = _students.Average(s => s.Average),
            MediaMaxima = _students.Max(s => s.Average),
            MediaMinima = _students.Min(s => s.Average),

        };
        return Ok(stats);
    }


    [HttpGet("stats-by-specialization")]
    public IActionResult GetStatsBySpecialization() {
        if(_students.Count == 0)
            return NotFound();
        var stats = _students
            .GroupBy(s => s.Specialization)
            .Select(group => new
            {
                Specialization = group.Key.ToString(),
                NumberOfStudents = group.Count(),
                Average = group.Average(s => s.Average),
                MinAverage = group.Min(s => s.Average),
                MaxAverage = group.Max(s => s.Average)
            });

        return Ok(stats);
    }

    [HttpGet("specializations")]
    public IActionResult GetSpecializations()
    {
        var specializations = _students
            .Select(s => s.Specialization.ToString())
            .Distinct()
            .OrderBy(s => s);

        return Ok(specializations);
    }

    [HttpGet("search")]

    public IActionResult GetSearch(string text, double? minAverage)
    {
        var query = _students
            .Where(s => s.Name.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase));
       
        if(minAverage is not null)
        {
            query = query.Where(s => s.Average >= minAverage);
        }

        query = query
            .OrderByDescending(s => s.Average)
            .ThenBy(s => s.Name);

        return Ok(query);
    }

    [HttpGet("page")]

    public IActionResult Page(int page=1, int pageSize = 3)
    {

        var result = _students
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Ok(result);

     
    }

    [HttpGet("top-specialization")]

    public IActionResult TopSpecialization()
    {
        var topSpecialization = _students
            .GroupBy(s => s.Specialization)
            .Select(group => new
            {
                Specialization = group.Key.ToString(),
                Average = group.Average(s => s.Average),

            })
            .OrderByDescending(elem => elem.Average).First();
       return Ok(topSpecialization);
    }

}