# EBookRental API 📚

## 簡介

這是一個基於 **ASP.NET Core** 開發的電子書租借系統後端 API。本專案實作了完整的會員機制、書籍搜尋、租借邏輯以及訂閱制會員系統。

## 製作動機

過去我的專案經驗都是與Machine Learning有關，編程能力幾乎只停在會使用基礎語法而已，所以在製作開始之前我有花蠻多的時間使用C++學習指標跟物件導向，
製作這個專案主要目標是「學習一個易維護、易除錯的開發架構，並培養良好的命名及標註習慣」。

## 專案結構

![image](https://github.com/Liang7414/EBookRental-Api/blob/master/Images/FileStruct.png)

## 🚀 技術棧 (Tech Stack)

* **技術顧問**: Gemini
* **開發框架**: .NET 8.0 / 9.0 (ASP.NET Core Web API)
* **資料庫存取**: Entity Framework Core (Code First)
* **資料庫**: MS SQL Server
* **身分驗證**: JWT (JSON Web Token) Authentication
* **物件映射**: AutoMapper (DTO Pattern)
* **開發工具**: Visual Studio 2022 / VS Code, Postman

## ✨ 核心功能與亮點

### 1. 訂閱與租借互斥邏輯 (Core Business Logic)
* **訂閱優先制**: 系統會自動判定使用者狀態。若使用者擁有有效的「訂閱方案」(Subscription)，系統將攔截並阻止其進行「單次租借」(Individual Rental)，避免不必要的消費支出。
* **續訂自動延展**: 實作了續訂邏輯。若使用者在到期日前續費，系統將自動從舊的 `EndDate` 開始往後累加，確保使用者權益。

### 2. 進階搜尋與效能優化
* **分頁查詢**: 實作 `BookSearchRequestDTO` 進行關鍵字搜尋，並結合分頁邏輯 (Skip & Take)，減少資料庫查詢負擔。
* **DTO 模式**: 嚴格遵守資料封裝原則，使用 `MappingProfile` 自動處理 Entity 與 ResponseDTO 之間的轉換，避免敏感資料 (如密碼) 外洩。

### 3. 安全與架構
* **ApiBaseController 設計**: 封裝常用的 `GetUserToken()` 方法，統一從 JWT Token 解析身分。
* **層次架構**: 採用 Controller -> Service -> Entity/Repository 的分層設計，確保程式碼具備良好的可測試性與維護性。

## 🛠️ 資料庫模型 (Database Schema)

* **Users**: 儲存使用者帳號資訊與加密密碼。
* **Books**: 包含書名、作者、分類及租借狀態。
* **Subscriptions**: 記錄使用者的訂閱起訖日。
* **IndividualRentals**: 記錄單本書籍的租借歷史與有效期。

### Swagger UI 預覽

![image](https://github.com/Liang7414/EBookRental-Api/blob/master/Images/Swagger-ui.png)
