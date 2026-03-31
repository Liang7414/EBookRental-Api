namespace EBookRental.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserType { get; set; } // 0: Admin, 1: User
        public DateTime CreatedAt { get; set; }

        // 關聯到訂閱與租借
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<IndividualRental> Rentals { get; set; } = new List<IndividualRental>();
    }
}

