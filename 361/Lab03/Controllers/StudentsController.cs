using Lab02.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Lab02.Controllers;

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
        if (id < 1)
            return BadRequest();

        Student? found = _students.FirstOrDefault(s => s.Id == id);
        if (found == null)
        {
            return NotFound();
        }
        return Ok(found);
    }

    [HttpPost]
    public IActionResult Create([FromBody] UpdateStudentRequest? student)
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

        Student updatedStudent = new Student();
        updatedStudent.Id = _students.Count > 0 ? _students.Max(s => s.Id) + 1 : 1;
        updatedStudent.Name = student.Name.Trim();
        updatedStudent.Average = student.Average;
        updatedStudent.Specialization = student.Specialization;

        _students.Add(updatedStudent);
        return CreatedAtAction(nameof(GetById), new { id = updatedStudent.Id }, updatedStudent);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be positive.");
        }

        Student? found = _students.FirstOrDefault(s => s.Id == id);
        if (found == null)
        {
            return NotFound();
        }
        _students.Remove(found);
        return NoContent();
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] Student student)
    {
        Student? updatedStudent = _students.FirstOrDefault(stud => stud.Id == student.Id);

        if (updatedStudent == null)
        {
            return NotFound();
        }

        updatedStudent.Name = student.Name;
        updatedStudent.Average = student.Average;
        updatedStudent.Specialization = student.Specialization;

        return Ok(updatedStudent);

    }

    [HttpGet("filter")]
    public IActionResult Filter([FromQuery] double? minAverage)
    {
        if (minAverage == null || minAverage < 1 || minAverage > 10)
        {
            return BadRequest("minAverage must be in range or not null.");
        }

        var filteredStudents = _students
            .Where(s => s.Average >= minAverage);

        return Ok(filteredStudents);
    }

    [HttpGet("top")]
    public IActionResult Top([FromQuery] double? minAverage)
    {
        if (minAverage == null || minAverage < 1 || minAverage > 10)
        {
            return BadRequest("minAverage must be in range or not null.");
        }

        var topStudents = _students
            .Where(s => s.Average <= minAverage)
            .OrderByDescending(s => s.Average);

        return Ok(topStudents);
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        bool allPassing = _students.All(s => s.Average >= 5);
        bool anyComputerScience = _students
            .Any(s => s.Specialization == Specialization.ComputerScience);

        var obj = new { anyComputerScience, totiTrecuti = allPassing };
        return Ok(obj);
    }

    [HttpGet("specializations")]
    public IActionResult GetSpecializations()
    {
        var specializations = _students
            .Select(s => s.Specialization.ToString())
            .Distinct()
            .OrderBy(s => s)
            .ToList();
        return Ok(specializations);
    }

    [HttpGet("stats-by-specialization")]
    public IActionResult StatsBySpecialization()
    {
        var stats = _students
            .GroupBy(s => s.Specialization)
            .Select(group => new
            {
                Name = group.Select(s => s.Specialization.ToString()).Distinct().First(),
                Count = group.Count(),
                Average = group.Average(student => student.Average),
                MinAverage = group.Min(student => student.Average),
                MaxAverage = group.Max(student => student.Average),
            });

        return Ok(stats);
    }

    [HttpGet("search")]
    public IActionResult Search(string text, double? minAverage)
    {
        var foundStudents = _students
            .Where(s => s.Name.Contains(text, StringComparison.OrdinalIgnoreCase));

        if (minAverage != null)
            foundStudents = foundStudents.Where(s => s.Average >= minAverage);

        foundStudents = foundStudents
            .OrderByDescending(s => s.Average)
            .ThenBy(s => s.Name);

        return Ok(foundStudents);
    }

    [HttpGet("page")]
    public IActionResult Page(int page = 1, int pageSize = 3)
    {
        var studentsPage = _students
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        if(studentsPage == null)
        {
            return BadRequest();
        }
        return Ok(studentsPage);
    }
}
