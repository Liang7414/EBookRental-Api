using EBookRental.Core.DTOs;
using EBookRental.Core.Entities;
using EBookRental.Core.Interfaces;
using EBookRental.Core.Setting;
using EBookRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace EBookRental.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly EBookRentalDbContext _context;
        private readonly IJwtSettings _jwtSettings;



        //沒使用到 AutoMapper，因為 UserService 目前只有註冊功能，沒有複雜的 DTO 轉換需求，所以不需要注入 IMapper。
        public UserService(EBookRentalDbContext context, IJwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        //註冊 Service
        public async Task<bool> RegisterAsync(UserRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username||u.Email==request.Email))
                return false;

            // 密碼雜湊 (Hashing)
            // BCrypt 會自動產生 Salt 並加密，存入資料庫的是一串亂碼
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            //建立 User Entity
            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash, // 資料庫欄位建議叫 PasswordHash
                Email = request.Email,
                CreatedAt = DateTime.Now,
                UserType = 0
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        //Login Service
        public async Task<string?> LoginAsync(UserLoginDto request)
        {
            // 1. 查詢資料庫
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return null;

            // 2. 使用 BCrypt 解碼/比對
            // 這裡是用 BCrypt 的靜態方法，傳入「明文」與「雜湊值」
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid) return null;

            // 3. 驗證成功，產生 JWT Token 
            return GenerateJwtToken(user);
        }

        // 產生 JWT Token 的方法
        private string GenerateJwtToken(User user)
        {
            // 1. 定義 Claims (聲明)：這些資訊會被加密打包在 Token 的 Payload 中
            // 注意：不要放敏感資訊（如密碼），因為 Payload 是可以被解碼讀取的
            var claims = new List<Claim>{
            // 使用內建的常數定義 Key 名稱，增加標準性
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        
            // 自定義 Claim：將用戶權限放入，方便前端或 API 判斷
            new Claim("UserType", user.UserType.ToString()),
        
            // 加入 Jti (JWT ID)，增加 Token 的唯一性，防止重放攻擊
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 2. 建立加密金鑰 (SymmetricSecurityKey)
            // 從我們定義的 IJwtSettings 介面中取得 Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            // 3. 選擇簽署演算法 (SigningCredentials)
            // 使用 HMAC SHA-512 進行數位簽章，確保 Token 內容不被篡改
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 4. 描述 Token 的規格 (SecurityTokenDescriptor)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),               // 放入身分資訊
                Expires = DateTime.Now.AddMinutes(30),              // 設定過期時間
                SigningCredentials = creds,                         // 放入簽署憑證
                Issuer = _jwtSettings.Issuer,                       // 設定發行者
                Audience = _jwtSettings.Audience                    // 設定受眾
            };

            // 5. 產出 Token 物件並轉化為字串
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 這就是最後回傳給前端的那串長字串
            return tokenHandler.WriteToken(token);
        }

    }
}

