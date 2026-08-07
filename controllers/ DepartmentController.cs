using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.DTOs;
using MyFirstApi.Models;
namespace MyFirstApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentController(AppDbContext context)
    {
        _context = context;
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await _context.Departments
            .Include(d => d.Employees)
            .ToListAsync();

        return Ok(new
        {
            Message = "Departments retrieved successfully.",
            Data = departments
        });
    }

       // GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return NotFound(new
            {
                Message = "Department not found."
            });
        }

        return Ok(new
        {
            Message = "Department retrieved successfully.",
            Data = department
        });
    }
        // CREATE
    [HttpPost]
    public async Task<IActionResult> CreateDepartment(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            DepartmentName = dto.DepartmentName
        };

        _context.Departments.Add(department);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Department created successfully.",
            Data = department
        });
    }
    // UPDATE
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, Department department)
    {
        if (id != department.Id)
        {
            return BadRequest(new
            {
                Message = "Department Id mismatch."
            });
        }

        _context.Entry(department).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Department updated successfully."
        });
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound(new
            {
                Message = "Department not found."
            });
        }

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Department deleted successfully."
        });
    }
}

