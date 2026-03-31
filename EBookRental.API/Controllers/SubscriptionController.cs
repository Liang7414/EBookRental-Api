using EBookRental.Core.DTOs;
using EBookRental.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBookRental.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ApiBaseController
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscriptionRequestDTO request)
        {
            try
            {
                // 透過 BaseController 的 GetUserId() 抓取 Token 裡的身分
                var userId = GetUserToken();

                var expiryDate = await _subscriptionService.CreateSubscriptionAsync(userId, request);

                if (expiryDate==null) return BadRequest("訂閱失敗");

                return Ok(new { message = "訂閱成功！歡迎享受全館閱讀。",
                                expiryDate = expiryDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetInfo")]
        public async Task<IActionResult> GetInfo()
        {
            var userId = GetUserToken();
            var info = await _subscriptionService.GetUserSubscriptionAsync(userId);

            if (info == null) return NotFound("目前沒有有效的訂閱。");

            return Ok(info);
        }

    }
}