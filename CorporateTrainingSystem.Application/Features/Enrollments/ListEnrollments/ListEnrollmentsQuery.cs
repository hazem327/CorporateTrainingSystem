using CorporateTrainingSystem.Domain.Entities;

namespace CorporateTrainingSystem.Application.Features.Enrollments.ListEnrollments
{
    public class ListEnrollmentsQuery
    {
        public int? EmployeeId { get; set; }
        public int? SessionId { get; set; }
        public EnrollmentStatus? Status { get; set; }
    }
}
