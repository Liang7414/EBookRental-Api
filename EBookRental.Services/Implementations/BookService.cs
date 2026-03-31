using AutoMapper;
using Azure.Core;
using EBookRental.Core.DTOs;
using EBookRental.Core.Entities;
using EBookRental.Core.Interfaces;
using EBookRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace EBookRental.Services.Implementations
{
    public class BookService : IBookService
    {
        //private readonly  _context "readonly"代表只能在建構子中被賦值，如果只有private而沒有readonly，則可以在類的其他方法中修改_context的值，這可能會導致不可預期的行為或錯誤。
        private readonly EBookRentalDbContext _context; 
        private readonly IMapper _mapper;
        public BookService(EBookRentalDbContext context ,IMapper mapper)
        {
            _context = context; // 透過相依注入 (DI) 取得 DbContext
            _mapper = mapper;  // 透過相依注入 (DI) 取得 AutoMapper 的 IMapper 實例
        }

        //Read 獲取所有書籍資訊
        public async Task<IEnumerable<BookResponseDTO>> GetAllBooksAsync()
        {
            var books = await _context.Books
                .Include(b => b.Category)            // 預先加載分類資料
                .ToListAsync();

            // 在 Service 內部完成轉換
            return _mapper.Map<IEnumerable<BookResponseDTO>>(books);
        }
        //Read 透過ID獲取特定書籍資訊
        public async Task<BookResponseDTO?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Category)             //預先加載分類資料
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            // automapper會在這裡正確執行轉換
            return _mapper.Map<BookResponseDTO>(book);
        }

        //Create 新增書籍資訊
        //直接接受BookEntity，因為前端傳來的資料結構與BookEntity相符，這樣可以簡化流程，不需要額外的DTO來接收資料。
        public async Task<BookResponseDTO> CreateBookAsync(Book book)
        {
            book.CreatedAt = DateTime.UtcNow; // 設定創建時間

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // 存檔後加載關聯資料，確保 DTO 能拿到分類名稱
            await _context.Entry(book).Reference(b => b.Category).LoadAsync();

            return _mapper.Map<BookResponseDTO>(book);
        }

        //Update 更新書籍資訊
        public async Task UpdateBookAsync(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        //Delete 刪除書籍資訊
        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        //Search 關鍵字搜尋書籍資訊
        // 這裡開始使用RequestDTO來接收前端的搜尋條件，並使用ResponseDTO來回傳搜尋結果
        public async Task<IEnumerable<BookResponseDTO>> SearchBooksAsync(BookSearchRequestDTO request)
        {
            //1. 建立查詢
            var query = _context.Books
                .Include(b => b.Category) // 重要：預先加載分類資料 (Eager Loading)
                .AsQueryable();

            // 2. 關鍵字過濾 (如果前端有傳關鍵字才過濾)
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
            query = query.Where(b => b.Title.Contains(request.Keyword) ||
                    b.Author.Contains(request.Keyword));
            }

            // 3. 分頁邏輯 (Skip & Take)
            // 假設第 2 頁，PageSize 10，則跳過前 (2-1)*10 = 10 筆，拿接下來的 10 筆
            var books = await query
                .OrderBy(b => b.Id) // 分頁前必須排序，否則資料順序會亂掉
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // 4. 轉換成 DTO 回傳
            return _mapper.Map<IEnumerable<BookResponseDTO>>(books);
        }


    }
}
