using Microsoft.AspNetCore.Mvc;
using System;

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
    public IActionResult GetById([FromRoute] int id)
    {
        Student? found = students.FirstOrDefault(s => s.Id == id);
        if (found == null)
        {
            return NotFound();
        }
        return Ok(found);
    }

    [HttpPost]
    public IActionResult Create(CreateStudentDTO? student)
    {
        if (student == null)
        {
            return BadRequest("Missing body.");
        }

        int newId = students.Max(s => s.Id) + 1;

        Student newStudent = new();
        newStudent.Id = newId;
        newStudent.Name = student.Name.Trim();
        newStudent.Average = student.Average;
        newStudent.Specialization = student.Specialization;
        students.Add(newStudent);
        return CreatedAtAction(nameof(GetById), new { id = newStudent.Id }, student);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive.");
        }

        Student? student = students.FirstOrDefault(s =>  id == s.Id);
     
        
        if (student == null)
        {
            return NotFound();
        }
        students.Remove(student);
        return NoContent();
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] Student student)
    {
        Student? updatedStudent = students.FirstOrDefault(s => s.Id == student.Id);
        if (updatedStudent == null)
        {
            return NotFound();
        }
        updatedStudent.Name = student.Name;
        updatedStudent.Average = student.Average;
        updatedStudent.Specialization = student.Specialization;
        return Ok(updatedStudent);
    }

    // Ex5
    [HttpGet("filter")]
    public IActionResult Filter([FromQuery] double? minAverage)
    {
        if(minAverage == null||minAverage < 1 || minAverage > 10)
        {
            return BadRequest("Nu se respecta conditiile pentru parametru");
        }

        List<Student> filteredStudents = students
            .Where(s => s.Average >= minAverage)
            .ToList();
        
        return Ok(filteredStudents);
    }

    //Ex6
    [HttpGet("top")]
    public IActionResult Top([FromQuery] double? minAverage)
    {
        if (minAverage == null || minAverage < 1 || minAverage > 10)
        {
            return BadRequest("Nu se respecta conditiile pentru parametru");
        }

        List<Student> filteredStudents = students
            .Where(s => s.Average >= minAverage)
            .OrderByDescending(s => s.Average)
            .ToList();


        return Ok(filteredStudents);
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        bool anyComputerScience = students
            .Any(s => s.Specialization == Specialization.ComputerScience);
        bool allPassing = students
            .All(s => s.Average >= 5);

        int totalStudents = students.Count();
        double totalAverage = students.Average(s => s.Average);
        double maxAverage = students.Max(s => s.Average);
        double minAverage = students.Min(s => s.Average);

        var result = new
        {
            totalStudents,
            totalAverage,
            maxAverage,
            minAverage,
            anyComputerScience,
            allPassing
        };

        return Ok(result);
    }

    //Ex 8
    [HttpGet("specializations")]
    public IActionResult GetSpecializations()
    {
        var specializations = students
            .Select(s => s.Specialization)
            .Distinct()
            .ToList();

        return Ok(specializations);

    }

    //Ex9
    [HttpGet("stats-by-specialization")]
    public IActionResult StatsBySpecialization()
    {
        var stats = students.GroupBy(s => s.Specialization)
            .Select(st => new
            {
                Count = st.Count(),
                Average = st.Average(s => s.Average),
                Min = st.Min(st => st.Average),
                Max = st.Max(st => st.Average)
            });

        return Ok(stats);
    }

    // Ex10
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string text, [FromQuery] double? minAverage)
    {
        var foundStudents = students
            .Where(s => s.Name.ToLower().Contains(text.ToLower().Trim()));

        if (minAverage != null)
            foundStudents = foundStudents
                .Where(s => s.Average >= minAverage);

        foundStudents = foundStudents.OrderByDescending(s => s.Average).ThenBy(s => s.Name);

        return Ok(foundStudents);
    }

    //Ex11

    [HttpGet("page")]

    public IActionResult Search(int page = 1 , int pageSize = 3)
    {
        if (page < 1 || students.Count / pageSize < page)
            return BadRequest("Data doesnt exists");

        var result = students
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return Ok(result);
    }



}