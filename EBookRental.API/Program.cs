using EBookRental.Core.Interfaces;
using EBookRental.Core.Setting;
using EBookRental.Infrastructure.Data;
using EBookRental.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 讀取 Jwt 設定，並綁定到 JwtSettings 類別
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
builder.Services.AddSingleton<IJwtSettings>(jwtSettings);

// 配置 JWT 驗證，指定使用 JwtBearer 認證方案
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

// 配置 JWT 驗證，設定 Token 的驗證參數
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,      // 設定是否驗證簽名金鑰，這是確保 Token 沒有被篡改的關鍵

        // 從設定讀取Key的格式，並使用 UTF8 編碼轉換成 byte[]，最後建立 SymmetricSecurityKey 物件
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)), 

        ValidateIssuer = true,                // 開啟Issuer驗證功能
        ValidIssuer = jwtSettings.Issuer,     // 從設定檔讀取Issuer的格式
        ValidateAudience = true,              // 開啟Audience驗證功能
        ValidAudience = jwtSettings.Audience, // 從設定檔讀取Audience的格式
        ValidateLifetime = true,              // 設定是否驗證Token的有效期
        ClockSkew = TimeSpan.Zero             // 緩衝時間設為 0，過期立即失效
    };
});


// 注入 DbContext，使用 SQL Server，連接字串從 appsettings.json 讀取
builder.Services.AddDbContext<EBookRentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注入 Service 層的服務
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// 注入 AutoMapper，自動掃描目前 AppDomain 中的所有組件來尋找 AutoMapper 的 Profile 類別
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 啟用認證中介，必須在 UseAuthorization 之前呼叫
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();
app.Run();
