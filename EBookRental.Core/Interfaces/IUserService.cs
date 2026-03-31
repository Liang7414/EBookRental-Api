using EBookRental.Core.DTOs;

namespace EBookRental.Core.Interfaces
{
    public interface IUserService
    {
        // 註冊：回傳布林值代表是否成功，或回傳 UserDto
        Task<bool> RegisterAsync(UserRegisterDto request);

        // 登入：成功則回傳 JWT Token 字串
        Task<string?> LoginAsync(UserLoginDto request);
    }
}