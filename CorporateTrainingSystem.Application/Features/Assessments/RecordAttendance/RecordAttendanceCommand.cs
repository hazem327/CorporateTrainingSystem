namespace CorporateTrainingSystem.Application.Features.Assessments.RecordAttendance
{
    public class RecordAttendanceCommand
    {
        public int EnrollmentId { get; set; }
        public bool IsPresent { get; set; }
        public string? Notes { get; set; }
    }
}