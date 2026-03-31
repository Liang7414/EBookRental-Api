namespace EBookRental.Core.DTOs
{
    public class RentalResponseDto
    {
        public int Id { get; set; }
        public string? BookTitle { get; set; } = string.Empty;
        public DateTime RentDate { get; set; }
        public DateTime ExpiryDate { get; set; } // 加一個月後的日期
        public string Status { get; set; } = string.Empty;

        public int RemainingDays=> (ExpiryDate - DateTime.Now).Days>0
                                   ?(ExpiryDate-DateTime.UtcNow).Days:0;
    }
}