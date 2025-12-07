namespace OcenaPracownicza.API.Views
{
    public class ManagerView
    {
        public required Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string AchievementsSummary { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public List<EmployeeView> Employees { get; set; } = new();
    }
}
