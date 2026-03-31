**新增遷移紀錄**



dotnet ef migrations add <描述這次改了什麼> --project EBookRental.Infrastructure --startup-project EBookRental.API



**--project：告訴工具 DbContext 的實作檔案在哪（在 Infrastructure）**

**--startup-project：告訴工具去哪裡讀取連線字串 appsettings.json（在 API）**





**根據紀錄修改資料庫屬性**



dotnet ef database update --project EBookRental.Infrastructure --startup-project EBookRental.API



**查看dotnet ef工具版本**


dotnet ef --version

