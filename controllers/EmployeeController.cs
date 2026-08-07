using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.DTOs;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Employee
    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .ToListAsync();

        return Ok(new
        {
            Message = "Employees retrieved successfully.",
            Data = employees
        });
    }

    // GET: api/Employee/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound(new
            {
                Message = $"Employee with Id {id} not found."
            });
        }

        return Ok(new
        {
            Message = "Employee retrieved successfully.",
            Data = employee
        });
    }

    // POST: api/Employee
    [HttpPost]
    public async Task<IActionResult> PostEmployee(CreateEmployeeDto dto)
    {
        var department = await _context.Departments.FindAsync(dto.DepartmentId);

        if (department == null)
        {
            return BadRequest(new
            {
                Message = "Invalid Department Id."
            });
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, new
        {
            Message = "Employee created successfully.",
            Data = employee
        });
    }

    // PUT: api/Employee/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmployee(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest(new
            {
                Message = "Employee Id does not match."
            });
        }

        var department = await _context.Departments.FindAsync(employee.DepartmentId);

        if (department == null)
        {
            return BadRequest(new
            {
                Message = "Invalid Department Id."
            });
        }

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Employees.Any(e => e.Id == id))
            {
                return NotFound(new
                {
                    Message = "Employee not found."
                });
            }

            throw;
        }

        return Ok(new
        {
            Message = "Employee updated successfully."
        });
    }

    // DELETE: api/Employee/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound(new
            {
                Message = "Employee not found."
            });
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Employee deleted successfully."
        });
    }
}