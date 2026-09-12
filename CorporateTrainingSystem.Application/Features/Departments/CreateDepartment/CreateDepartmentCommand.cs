namespace CorporateTrainingSystem.Application.Features.Departments.CreateDepartment
{
    public class CreateDepartmentCommand
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}