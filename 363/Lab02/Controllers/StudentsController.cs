using Microsoft.AspNetCore.Mvc;
using System;

namespace Lab02_Demo.Controllers;

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
    public IActionResult Create(Student student)
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
    public IActionResult Update([FromBody] Student student)
    {
        if (student.Id <= 0)
        {
            return BadRequest("Id must be positive.");
        }

        Student? updatedStudent = null;
        for (int i = 0; i < students.Count; i++)
        {
            if (students[i].Id == student.Id)
            {
                updatedStudent = students[i];
                break;
            }
        }
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

        List<Student> filteredStudents = [];
        foreach(Student s in students)
        {
            if (s.Average >= minAverage)
                filteredStudents.Add(s);
        }
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

        List<Student> filteredStudents = [];
        foreach (Student s in students)
        {
            if (s.Average >= minAverage)
                filteredStudents.Add(s);
        }

        for(int i = 0; i < filteredStudents.Count - 1; i++)
        {
            for(int j = i + 1; j < filteredStudents.Count; j++)
            {
                if (filteredStudents[i].Average < filteredStudents[j].Average)
                {
                    Student aux = filteredStudents[i];
                    filteredStudents[i] = filteredStudents[j];
                    filteredStudents[j] = aux;
                }
            }
        }

        return Ok(filteredStudents);
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        bool anyComputerScience = false;
        foreach(var s in students)
        {
            if(s.Specialization == Specialization.ComputerScience)
            {
                anyComputerScience = true;
                break;
            }
        }

        bool allPassing = true;
        foreach (var s in students)
        {
            if (s.Average < 5)
            {
                allPassing = false;
                break;
            }
        }

        return Ok(new {AnyComputerScience = anyComputerScience, Trecuti = allPassing});
    }

}


