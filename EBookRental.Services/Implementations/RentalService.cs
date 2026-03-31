using AutoMapper;
using EBookRental.Core.DTOs;
using EBookRental.Core.Entities;
using EBookRental.Core.Interfaces;
using EBookRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace EBookRental.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly EBookRentalDbContext _context;
        private readonly IMapper _mapper;

        // 建構子，注入 DbContext 和 AutoMapper
        public RentalService(EBookRentalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //方法定義：租書邏輯，包含加一個月的核心邏輯
        public async Task<RentalResponseDto> RentBookAsync(int userId,RentalRequestDto request)
        {
            //檢查書是否存在 (邏輯檢查)
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null) throw new Exception("找不到該書籍");

            //【核心檢查】檢查該用戶是否有訂閱，因商業邏輯設定訂閱者可無限制閱讀
            var SubActive = await _context.Subscriptions
                            .AnyAsync(s=>s.UserId==userId && s.EndDate>=DateTime.UtcNow);
            if (SubActive) throw new Exception("您已完成訂閱，不需要額外租借!!");

            //【核心檢查】檢查該用戶是否已持有此書且尚未過期
            // 使用 AnyAsync 效率最高，只要找到一筆符合條件的就回傳 true
            bool isAlreadyRented = await _context.Rentals.AnyAsync(r =>
                r.UserId == userId &&
                r.BookId == request.BookId &&
                r.ExpiryDate > DateTime.UtcNow); // 檢查過期時間是否大於現在

            if (isAlreadyRented)
            {
                // 這裡可以拋出一個自定義例外，或者回傳特定的錯誤訊息
                throw new Exception("您已租借過此書，且仍在有效期內。");
            }

            

            //建立租借紀錄 Entity
            var rental = new IndividualRental
            {
                BookId = request.BookId,
                UserId = userId,
                RentDate = DateTime.UtcNow,
                // 核心邏輯：設定一個月後過期
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            //存入資料庫
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            //為了讓 DTO 能顯示書名，加載 Book 資訊
            await _context.Entry(rental).Reference(r => r.Book).LoadAsync();

            //透過 AutoMapper 轉換並回傳
            return _mapper.Map<RentalResponseDto>(rental);
        }

        //方法定義：取得用戶的租借紀錄，包含書名等資訊
        public async Task<IEnumerable<RentalResponseDto>> GetRentalsByUserAsync(int userId)
        {
            // 1. 從資料庫撈取資料
            var rentals = await _context.Rentals
                .Include(r => r.Book)               /* 預先加載書籍資訊，這樣 DTO 才有書名
                                                       因為DbContext有給導覽屬性Rental才能找到Book*/ 
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RentDate) // 按租借時間排序，最新的在前面
                .ToListAsync();

            // 2. 透過 AutoMapper 轉換成 DTO 清單
            return _mapper.Map<IEnumerable<RentalResponseDto>>(rentals);
        }


    }
}