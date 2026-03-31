namespace EBookRental.Core.DTOs
{
    public class BookResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal RentalPrice { get; set; }

        // 核心：用字串代替整個 Category 物件
        public string CategoryName { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}