namespace EBookRental.Core.Entities
{
    public class IndividualRental
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        // 導覽屬性
        public User? User { get; set; }
        public Book? Book { get; set; }

        // 判斷是否過期
        public bool IsExpired => ExpiryDate < DateTime.Now;
    }
}