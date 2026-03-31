namespace EBookRental.Core.DTOs
{
    public class SubscriptionRequestDTO
    {
        // 方案類型：1 = 月繳(30天), 2 = 季繳(90天), 3 = 年繳(365天)
        public int PlanType { get; set; }
    }
}