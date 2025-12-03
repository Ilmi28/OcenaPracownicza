namespace OcenaPracownicza.API.Views
{
    public class AdminView
    {
        public required Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
