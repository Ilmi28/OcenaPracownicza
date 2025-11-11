namespace OcenaPracownicza.API.Entities
{
    public class Employee : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Position { get; set; }
        public required string Department { get; set; }
    }
}
