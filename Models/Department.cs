namespace MyFirstApi.Models;

public class Department
{
    public int Id { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    // Navigation Property
    public List<Employee> Employees { get; set; } = new();
}