using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Responses
{
    public class EmployeeResponse
    {
        public EmployeeResponse() { }

        public EmployeeResponse(Employee e)
        {
            Id = e.Id;
            FirstName = e.FirstName;
            LastName = e.LastName;
            Position = e.Position;
            Unit = e.Unit;
            AcademicTitle = e.AcademicTitle;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Unit { get; set; }
        public string AcademicTitle { get; set; }
    }
}
