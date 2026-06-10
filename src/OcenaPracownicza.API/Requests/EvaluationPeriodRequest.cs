namespace OcenaPracownicza.API.Requests;

public class EvaluationPeriodRequest
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string RegulationVersion { get; set; } = null!;
    public bool IsClosed { get; set; }
}