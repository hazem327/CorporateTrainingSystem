namespace CorporateTrainingSystem.Application.Features.Employees.ListEmployees
{
    public class EmployeeListResult
    {
        public List<EmployeeListItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}