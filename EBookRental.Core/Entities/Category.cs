namespace EBookRental.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        // 導覽屬性
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
