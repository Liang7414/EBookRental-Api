namespace EBookRental.Core.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal RentalPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        // 外鍵與導覽屬性
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
