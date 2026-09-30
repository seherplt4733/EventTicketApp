using System.ComponentModel.DataAnnotations;

namespace EventTicketApp.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Etkinlik adı zorunludur.")]
        [Display(Name = "Etkinlik Adı")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Tarih")]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        [Display(Name = "Konum")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bilet fiyatı zorunludur.")]
        [Display(Name = "Bilet Fiyatı")]
        public decimal Price { get; set; }

        // --- YENİ EKLENEN KONTENJAN ALANLARI ---
        [Display(Name = "Toplam Kontenjan")]
        public int Capacity { get; set; } = 100;

        [Display(Name = "Kalan Bilet")]
        public int AvailableSeats { get; set; } = 100;

        // Yabancı Anahtar (Foreign Key)
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Afiş / Resim")]
        public string? ImagePath { get; set; }
    }
}