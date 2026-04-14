namespace OcenaPracownicza.API.Entities
{
    public class EvaluationPeriod : BaseEntity
    {
        public string Name { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Regulation { get; set; } = default!;
        public bool IsActive { get; set; }
        public ICollection<EvaluationCriterion> Criteria { get; set; } = new List<EvaluationCriterion>();
    }
}
