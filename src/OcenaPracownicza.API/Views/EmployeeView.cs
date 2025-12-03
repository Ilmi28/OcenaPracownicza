namespace OcenaPracownicza.API.Views
{
    public class EmployeeView
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string Period { get; set; } = null!;
        public string FinalScore { get; set; } = null!;
        public string AchievementsSummary { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
