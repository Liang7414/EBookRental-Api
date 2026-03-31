namespace EBookRental.Core.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // 導覽屬性：這筆訂閱屬於哪個使用者
        public User? User { get; set; }

        // 商業邏輯屬性：判斷是否還在效期內 (唯讀)
        public bool IsActive => EndDate >= DateTime.Now;
    }
}