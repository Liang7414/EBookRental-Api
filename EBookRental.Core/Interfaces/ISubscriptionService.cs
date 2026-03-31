using EBookRental.Core.DTOs;

namespace EBookRental.Core.Interfaces
{
    public interface ISubscriptionService
    {
        Task<DateTime?> CreateSubscriptionAsync(int userId, SubscriptionRequestDTO request);
        Task<SubscriptionResponseDTO?> GetUserSubscriptionAsync(int userId);
    }
}
