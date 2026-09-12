namespace CorporateTrainingSystem.Application.Features.Employees.ListEmployees
{
    public class EmployeeListQuery
    {
        public string? SearchTerm { get; set; }
        public int? DepartmentId { get; set; }
        public bool? IsActive { get; set; }
        public string SortBy { get; set; } = "Name";      // Name, EmployeeNumber, Department, HireDate
        public bool SortDescending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}