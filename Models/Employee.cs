using System.Text.Json.Serialization;

namespace MyFirstApi.Models;

public class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    // Foreign Key
    public int DepartmentId { get; set; }

    // Navigation Property
    [JsonIgnore]

    public Department? Department { get; set; }
    
}       