using Microsoft.AspNetCore.Mvc;

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
    public IActionResult GetById([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive.");
        }
        Student? found = null;
        foreach (var s in students)
        {
            if (s.Id == id)
            {
                found = s;
                break;
            }
        }
        if (found == null)
        {
            return NotFound();
        }
        return Ok(found);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Student? student)
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
        if (student.Average < 1 || student.Average > 10)
        {
            return BadRequest("Average must be between 1 and 10.");
        }
        int newId = 1;
        foreach (var s in students)
        {
            if (s.Id >= newId)
            {
                newId = s.Id + 1;
            }
        }
        student.Id = newId;
        student.Name = student.Name.Trim();
        students.Add(student);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive.");
        }
        int index = -1;
        for (int i = 0; i < students.Count; i++)
        {
            if (students[i].Id == id)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            return NotFound();
        }
        students.RemoveAt(index);
        return NoContent();
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] Student? student)
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
        if (student.Average < 1 || student.Average > 10)
        {
            return BadRequest("Average must be between 1 and 10.");
        }
        int index = -1;
        for (int i = 0; i < students.Count; i++)
        {
            if (students[i].Id == student.Id)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            return NotFound();
        }
        students[index].Name = student.Name;
        students[index].Specialization = student.Specialization;
        students[index].Average = student.Average;
        return Ok(student);
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

        foreach (var student in students)
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
        bool anyComputerScience = false;
        bool allPassing = true;

        foreach(var s in students)
        {
            if(s.Average < 5)
            {
                allPassing = false;
                break;
            }
        }
        foreach(var s in students)
        {
            if(s.Specialization == Specialization.ComputerScience)
            {
                anyComputerScience = true;
                break;
            }
        }

        var stats = new
        {
            AnyComputerScience = anyComputerScience,
            TotiTrecuti = allPassing,
        };
        return Ok(stats);
    }

}