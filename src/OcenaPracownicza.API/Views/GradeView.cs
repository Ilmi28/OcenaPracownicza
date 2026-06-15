namespace OcenaPracownicza.API.Views;

public class GradeView
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string? Comment { get; set; }
    public Guid EmployeeId { get; set; }
    public string PeriodName { get; set; } = null!;
}