using AutoMapper;
using EBookRental.Core.DTOs;
using EBookRental.Core.Entities;
using EBookRental.Core.Interfaces;
using EBookRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EBookRental.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly EBookRentalDbContext _context;
        private readonly IMapper _mapper;

        public SubscriptionService(EBookRentalDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DateTime?> CreateSubscriptionAsync(int userId, SubscriptionRequestDTO request)
        {
            // 1. 根據方案決定增加的天數 (使用 C# 8.0+ Switch Expression)
            int MonthsToAdd = request.PlanType switch
            {
                1 => 1,   // 月
                2 => 3,   // 季
                3 => 12,  // 年
                _ => throw new Exception("請選擇有效的訂閱方案")
            };

            // 2. 檢查該使用者是否已有「有效的」訂閱 (續訂邏輯)
            // 找出最晚過期的那一筆
            var latestSubscription = await _context.Subscriptions
                .Where(s => s.UserId == userId && s.EndDate > DateTime.Now)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            DateTime newStartDate;
            DateTime newEndDate;

            if (latestSubscription != null)
            {
                // 情況 A：續訂。從舊的 EndDate 往後加
                newStartDate = latestSubscription.StartDate; // 沿用原本的起始日
                newEndDate = latestSubscription.EndDate.AddMonths(MonthsToAdd);

                // 直接更新該筆紀錄的過期日
                latestSubscription.EndDate = newEndDate;
            }
            else
            {
                // 情況 B：新訂閱。從現在開始算
                newStartDate = DateTime.UtcNow;
                newEndDate = DateTime.UtcNow.AddMonths(MonthsToAdd);

                // insert 新的訂閱紀錄
                var newSub = new Subscription
                {
                    UserId = userId,
                    StartDate = newStartDate,
                    EndDate = newEndDate
                };
                _context.Subscriptions.Add(newSub);
            }
            var success = await _context.SaveChangesAsync() > 0;
            return success ? newEndDate : null;
        }

        public async Task<SubscriptionResponseDTO?> GetUserSubscriptionAsync(int userId)
        {
            // 找出該使用者目前「有效的」訂閱
            var subscription = await _context.Subscriptions
                .Where(s => s.UserId == userId && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();
            // 如果沒有有效訂閱，回傳 null
            if (subscription == null) return null;

            return _mapper.Map<SubscriptionResponseDTO>(subscription);
        }
    }
}
