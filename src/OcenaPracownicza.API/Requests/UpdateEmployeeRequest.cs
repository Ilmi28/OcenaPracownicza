namespace OcenaPracownicza.API.Requests
{
    public class UpdateEmployeeRequest
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string Period { get; set; } = null!;
        public string FinalScore { get; set; } = null!;
        public string AchievementsSummary { get; set; } = null!;
    }
}
