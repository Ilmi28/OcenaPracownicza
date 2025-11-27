namespace OcenaPracownicza.API.Entities
{
    public class Employee : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Position { get; set; }
        public string Unit { get; set; }
        public string AcademicTitle { get; set; }
        public required string Period { get; set; }
        public required string FinalScore { get; set; }
        public required string AchievementsSummary { get; set; }
    }
}
