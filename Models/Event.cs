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

        [Display(Name = "Afiş / Resim")]
        public string? ImagePath { get; set; }

        // --- KONTENJAN VE BİLET YÖNETİMİ ---
        [Display(Name = "Toplam Kontenjan")]
        [Range(1, 10000, ErrorMessage = "Kontenjan en az 1 olmalıdır.")]
        public int Capacity { get; set; } = 100;

        [Display(Name = "Satılan Bilet Sayısı")]
        public int SoldTicketsCount { get; set; } = 0;

        // Kalan bilet sayısını hesaplayan property
        [Display(Name = "Kalan Bilet")]
        public int AvailableSeats => Capacity - SoldTicketsCount;

        // Yabancı Anahtar (Foreign Key)
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}