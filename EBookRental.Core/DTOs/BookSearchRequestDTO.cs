namespace EBookRental.Core.DTOs
{
    public class BookSearchRequestDTO
    {
        public string? Keyword { get; set; }  // 關鍵字 (書名或作者)
        public int PageNumber { get; set; } = 1; // 第幾頁 (預設第1頁)
        public int PageSize { get; set; } = 10;  // 每頁幾筆 (預設10筆)
    }
}
