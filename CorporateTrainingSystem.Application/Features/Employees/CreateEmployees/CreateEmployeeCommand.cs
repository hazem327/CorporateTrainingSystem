namespace CorporateTrainingSystem.Application.Features.Employees.CreateEmployee
{
    public class CreateEmployeeCommand
    {
        public string EmployeeNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.Today;
        public int DepartmentId { get; set; }
    }
}