namespace EBookRental.Core.DTOs
{
    public class SubscriptionResponseDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        // 額外加個小功能：計算剩餘天數
        public int DaysRemaining => (EndDate - DateTime.Now).Days;
    }
}
