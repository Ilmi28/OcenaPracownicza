namespace OcenaPracownicza.API.Requests;

public class CreateGradeRequest
{
    public decimal Value { get; set; }
    public string? Comment { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid EvaluationPeriodId { get; set; }
}