namespace CorporateTrainingSystem.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        public int PassingScore { get; set; }         
        public int CertificateValidityMonths { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
        public ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    }
}