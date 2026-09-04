namespace CorporateTrainingSystem.Application.Features.Enrollments.EnrollEmployee
{
    public class EnrollEmployeeResult
    {
        public bool Success { get; set; }
        public int? EnrollmentId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
