using System;

namespace OcenaPracownicza.API.Responses
{
    public class AchievementElementResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public int Activity { get; set; }
        public int Department { get; set; }
        public int Category { get; set; }
        public string ActivityName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;

        public string Name { get; set; } = null!;
        public decimal BasePoints { get; set; }
        public bool IsStackable { get; set; }

        public Guid EvaluationPeriodId { get; set; }
        public string EvaluationPeriodName { get; set; } = null!;
    }
}