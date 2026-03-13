namespace OcenaPracownicza.API.Entities;

// Modele sa reprezentacja tabel w bazie danych
public class ExampleEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SomeDetail { get; set; } = string.Empty;
}
