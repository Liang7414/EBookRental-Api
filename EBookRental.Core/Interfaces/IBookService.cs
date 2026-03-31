using EBookRental.Core.DTOs;
using EBookRental.Core.Entities;

namespace EBookRental.Core.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponseDTO>> GetAllBooksAsync();
        // 讀取功能
        Task<BookResponseDTO?> GetBookByIdAsync(int id);
        // 新增功能
        Task<BookResponseDTO> CreateBookAsync(Book book);
        // 修改功能
        Task UpdateBookAsync(Book book);
        // 刪除功能
        Task DeleteBookAsync(int id);
        // 搜尋功能
        Task<IEnumerable<BookResponseDTO>> SearchBooksAsync(BookSearchRequestDTO request);
    }
}