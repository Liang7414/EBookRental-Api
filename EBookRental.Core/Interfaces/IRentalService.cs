using EBookRental.Core.DTOs;

namespace EBookRental.Core.Interfaces
{
    public interface IRentalService
    {
        // 租一本書
        Task<RentalResponseDto> RentBookAsync(int userId,RentalRequestDto request);
        // 取得使用者的租借紀錄
        Task<IEnumerable<RentalResponseDto>> GetRentalsByUserAsync(int userId);
    }
}
