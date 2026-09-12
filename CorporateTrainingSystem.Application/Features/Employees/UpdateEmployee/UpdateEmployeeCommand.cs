namespace CorporateTrainingSystem.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeCommand
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
    }
}