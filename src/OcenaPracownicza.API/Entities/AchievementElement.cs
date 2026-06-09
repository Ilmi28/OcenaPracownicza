namespace OcenaPracownicza.API.Entities
{
    public class AchievementElement : BaseEntity
    {
        public string Code { get; set; } = null!;
        public int ActivityId { get; set; }
        public int DepartmentId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public decimal BasePoints { get; set; }
        public bool IsStackable { get; set; }
        public Guid EvaluationPeriodId { get; set; }
        public EvaluationPeriod EvaluationPeriod { get; set; } = null!;
    }
    public static class AchievementDictionary
    {
        public record Definition(int Id, string Label);

        public static readonly List<Definition> Activities = new() {
        new(0, "Działalność Naukowo-Badawcza"),
        new(1, "Działalność Dydaktyczno-Organizacyjna")
        };

        public static readonly List<Definition> Departments = new() {
        new(0, "Dział Spraw Personalnych"),
        new(1, "Dział Nauki"),
        new(2, "Dział Informacji Naukowej Biblioteki PB"),
        new(3, "Biuro ds. Rozwoju i Programów Międzynarodowych"),
        new(4, "Ośrodek Własności Intelektualnej"),
        new(5, "Biuro ds. Współpracy Międzynarodowej"),
        new(6, "Centrum Spraw Studenckich, Dydaktyki i Rekrutacji"),
        new(7, "Dział Jakości Kształcenia"),
        new(8, "Dział Promocji")
        };

        public static readonly List<Definition> Categories = new() {
        new(0, "Inne osiągnięcia"),
        new(1, "Podnoszenie jakości kształcenia"),
        new(2, "Pełnienie funkcji (za każdy rok)"),
        new(3, "Nagrody i wyróżnienia"),
        new(4, "Rozwój naukowy"),
        new(5, "Projekty obejmujące badania naukowe i prace rozwojowe realizowane w Uczelni"),
        new(6, "Organizacja konferencji i wypraw naukowych/dydaktycznych"),
        new(7, "Recenzje"),
        new(8, "Działalność naukowa"),
        new(9, "Sztuki plastyczne i konserwacja dzieł sztuki"),
        new(10, "Publikacje dydaktyczne"),
        new(11, "Patenty na wynalazki, prawa ochronne na wzory użytkowe i wyłączne prawa hodowców do odmian roślin"),
        new(12, "Projekty obejmujące badania naukowe i prace rozwojowe"),
        new(13, "Zajęcia w języku obcym, wykłady za granicą"),
        new(14, "Organizacja i udział w wydarzeniach o charakterze promocyjnym, popularno-naukowym i edukacyjnym, budujących pozytywny wizerunek Uczelni")

        };
    }
}
