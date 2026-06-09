using System;
using System.ComponentModel.DataAnnotations;

namespace OcenaPracownicza.API.Requests
{
    public class AddUpdateAchievementElementRequest
    {
        [Required(ErrorMessage = "Kod osiągnięcia jest wymagany.")]
        [MaxLength(50, ErrorMessage = "Kod nie może przekraczać 50 znaków.")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Rodzaj działalności jest wymagany.")]
        [Range(0, 3, ErrorMessage = "Nieprawidłowy rodzaj działalności.")]
        public int Activity { get; set; }

        [Required(ErrorMessage = "Dział weryfikujący jest wymagany.")]
        [Range(0, 999, ErrorMessage = "Nieprawidłowy dział.")]
        public int Department { get; set; }

        [Required(ErrorMessage = "Kategoria osiągnięcia jest wymagana.")]
        [Range(0, 999, ErrorMessage = "Nieprawidłowa kategoria.")]
        public int Category { get; set; }

        [Required(ErrorMessage = "Nazwa osiągnięcia jest wymagana.")]
        [MaxLength(500, ErrorMessage = "Nazwa osiągnięcia nie może przekraczać 500 znaków.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Punkty bazowe są wymagane.")]
        [Range(0, 1000, ErrorMessage = "Punkty bazowe muszą być wartością między 0 a 1000.")]
        public decimal BasePoints { get; set; }

        public bool IsStackable { get; set; }

        [Required(ErrorMessage = "Okres oceny jest wymagany.")]
        public Guid EvaluationPeriodId { get; set; }
    }
}