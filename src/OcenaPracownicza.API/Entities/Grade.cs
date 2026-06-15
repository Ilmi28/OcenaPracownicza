namespace OcenaPracownicza.API.Entities
{
    public class Grade : BaseEntity
    {
        public decimal Value { get; set; }
        public string? Comment { get; set; } 

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public Guid EvaluationPeriodId { get; set; }
        public EvaluationPeriod EvaluationPeriod { get; set; } = null!;

        public Guid RatedByUserId { get; set; }
    }
}
