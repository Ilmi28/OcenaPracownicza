using System.Text.Json.Serialization;

namespace OcenaPracownicza.API.Entities
{
    public class EvaluationCriterion : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int MinimumScore { get; set; }
        public int EvaluationPeriodId { get; set; }
        public EvaluationPeriod? EvaluationPeriod { get; set; }
    }
}
