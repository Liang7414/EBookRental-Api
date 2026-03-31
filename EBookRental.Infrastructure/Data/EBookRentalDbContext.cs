using EBookRental.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EBookRental.Infrastructure.Data
{
    public class EBookRentalDbContext : DbContext
    {
        // 建構子，接受 DbContextOptions 以便配置資料庫連線等設定
        public EBookRentalDbContext(DbContextOptions<EBookRentalDbContext> options) : base(options)
        {
        }

        // 宣告資料表 (DbSet)
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<IndividualRental> Rentals { get; set; }

        //  複寫SaveChangesAsync 新增自動設定 CreatedAt 欄位的功能
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 找出所有狀態為 Added (新增) 的 Entity
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                // 如果這個 Entity 有 CreatedAt 這個屬性，就自動幫它填入時間
                var property = entry.Entity.GetType().GetProperty("CreatedAt");
                
                property?.SetValue(entry.Entity, DateTime.Now);
                
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        // 配置模型關聯 (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 呼叫基底方法
            base.OnModelCreating(modelBuilder);

            // 1. 設定 Book 與 Category 的一對多關係
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)            // Book 實際有一個 Category屬性
                .WithMany(c => c.Books)             /* 一個Category擁有多個 Books，因為 Category 需要知道有哪些書籍屬於它
                                                       所以設定反向導航屬性*/
                .HasForeignKey(b => b.CategoryId);  // Book搜索到Category的外鍵是 CategoryId

            // 2. 設定精確度 (避免 Decimal 警告)
            modelBuilder.Entity<Book>()
                .Property(b => b.RentalPrice)       // Book 的 RentalPrice 屬性
                .HasPrecision(18, 2);               // 設定為 Decimal(18, 2)，即總共 18 位數，其中小數點後 2 位

            // 3. 設定 Subscription 與 User 的關係
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.User)                // Subscription實際有一個User屬性
                .WithMany(u => u.Subscriptions)     /* 一個User擁有多個Subscription，因User需要知道有哪些訂閱紀錄
                                                       故設定反向導航屬性*/
                .HasForeignKey(s => s.UserId);      // Subscription搜索到User的外鍵是 UserId

            // 4. 設定 IndividualRental 與 User/Book 的關係
            modelBuilder.Entity<IndividualRental>()
                .HasOne(r => r.User)                // IndividualRental實際有一個User屬性
                .WithMany(u => u.Rentals)           /* 一個User擁有多個Rentals屬性，因User需要知道有哪些租借紀錄
                                                       故設定反向導航屬性*/
                .HasForeignKey(r => r.UserId);      // IndividualRental搜索到User的外鍵是 UserId

            modelBuilder.Entity<IndividualRental>()
                .HasOne(r => r.Book)                // IndividualRental實際有一個Book屬性
                .WithMany()                         // Book不需要知道有哪些租借紀錄，所以不設定反向導航屬性
                .HasForeignKey(r => r.BookId);      // IndividualRental搜索到Book的外鍵是 BookId

            // 5. 設定 User 的屬性限制

            modelBuilder.Entity<User>(entity =>
            {
                // 設定 Username 為必填，長度最大 50
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                // 設定 PasswordHash 的長度 (BCrypt 固定產出約 60 字元，建議給 255 比較保險)
                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                // 設定 Email 為必填，長度最大 100
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // 如果你想要建立唯一索引 (不允許重複的 Email)
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });


        }
    }
}
