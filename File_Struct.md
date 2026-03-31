EBookRentalSystem/ (Solution)
├── EBookRental.API/              # ASP.NET Core Web API 專案 (入口層)
│   ├── Controllers/              # 負責接收 HTTP 請求 (如 BookController.cs)
│   ├── Program.cs                # 配置服務與中間件管道
│   └── appsettings.json          # 資料庫連線字串配置
│
├── EBookRental.Core/             # Class Library 專案 (核心層)
│   ├── Entities/                 # 對應資料庫的類別 (User, Book, Category)
│   ├── DTOs/                     # 資料傳輸物件 (BookResponseDto, LoginDto)
│   ├── Interfaces/               # 定義 Service 與 Repository 的介面
│   └── Settings/                 # 新增：設定檔的「定義」
│        ├── IJwtSettings.cs       # 介面：定義需要哪些設定
│        └── JwtSettings.cs        # 實作：對應 JSON 結構的類別│
│
├── EBookRental.Services/         # Class Library 專案 (業務邏輯層)
│   ├── Implementations/          # 實作核心邏輯 (如判斷租借是否過期)
│   └── Mappings/                 # AutoMapper 設定 (類別轉換邏輯)
│
├── EBookRental.Infrastructure/   # Class Library 專案 (基礎設施層)
    ├── Data/                     # DbContext (EF Core 資料庫上下文)
    └── Migrations/               # 資料庫遷移紀錄檔
