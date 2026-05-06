namespace OcenaPracownicza.API.Entities;

public class EvaluationPeriod : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string RegulationVersion { get; set; } = null!;
    public bool IsClosed { get; set; } = false;
    public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
}