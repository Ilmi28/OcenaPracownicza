using System.ComponentModel.DataAnnotations;

namespace OcenaPracownicza.API.Requests
{
    public class EvaluationPeriodRequest
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RegulationVersion { get; set; }
    }
}
