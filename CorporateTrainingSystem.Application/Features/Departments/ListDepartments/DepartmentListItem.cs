namespace CorporateTrainingSystem.Application.Features.Departments.ListDepartments
{
    public class DepartmentListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EmployeeCount { get; set; }
    }
}