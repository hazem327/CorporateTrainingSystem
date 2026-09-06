namespace CorporateTrainingSystem.Application.Features.Assessments.RecordAssessmentResult
{
    public class RecordAssessmentResultResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public bool Passed { get; set; }
    }
}