using EBookRental.Core.Entities;
using EBookRental.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EBookRental.Core.DTOs;

namespace EBookRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // 這會讓網址變成 api/book
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // READ: 取得所有書籍 (GET: api/book)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books); // 回傳 200 OK 與書籍清單
        }

        // READ: 取得單一書籍 (GET: api/book/5)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        // CREATE: 新增書籍 (POST: api/book)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdBook = await _bookService.CreateBookAsync(book);
            // 回傳 201 Created，並在 Header 附上取得該資源的 Location
            return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
        }

        // UPDATE: 修改書籍 (PUT: api/book/5)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (id != book.Id) return BadRequest("URL 中的 ID 與資料內容不符");

            try
            {
                await _bookService.UpdateBookAsync(book);
                return NoContent(); // 204 No Content，代表成功且不需回傳主體
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新失敗: {ex.Message}");
            }
        }

        // DELETE: 刪除書籍 (DELETE: api/book/5)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
        // Read: 搜尋書籍(GET: api/book/search?Keyword=C%23&PageNumber=1&PageSize=10)
        //PageNumber和PageSize有預設值，若URL沒有提供則使用預設值
        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] BookSearchRequestDTO request)
        {
            // [FromQuery] 會自動把 URL 的參數 (如 ?Keyword=C#&PageNumber=1) 綁定到 DTO
            var results = await _bookService.SearchBooksAsync(request);
            return Ok(results);
        }
    }
}