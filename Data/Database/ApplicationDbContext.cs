using Data.Database.Table;
using Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BlogWebsite.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamHistory> ExamHistories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamHistoryDetails> ExamHistoryDetails { get; set; }

        public DbSet<Save> Saves { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostSave> PostSaves { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<PaidPost> PaidPosts { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ReplyResponse> ReplyResponses { get; set; }
		public DbSet<PaymentRequest> PaymentRequests { get; set; }
		public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<PostView> PostViews { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=ADMIN-PC\\SQLEXPRESS01;Database=DATN;Trusted_Connection=True; TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Flower>()
            .HasKey(o => new { o.IdUser, o.IdFlower });


            modelBuilder.Entity<PostTag>().HasKey(u => new
            {
                u.IdTag,
                u.IdPost
            });

            modelBuilder.Entity<PostSave>().HasKey(u => new
            {
                u.IdSave,
                u.IdPost
            });


            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(e => e.TagName).IsUnique();
            });
            modelBuilder.Entity<Exam>()
    .HasOne(n => n.User)
    .WithMany(u => u.Exams)
    .HasForeignKey(n => n.IdUser)
    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExamHistory>()
                .HasOne(n => n.User)
                .WithMany(u => u.ExamHistories)
                .HasForeignKey(n => n.IdUser)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notice>()
                   .HasOne(n => n.UserReceive)
                   .WithMany(u => u.NoticesReceived)
                   .HasForeignKey(n => n.ToUser)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notice>()
                .HasOne(n => n.UserSend)
                .WithMany(u => u.NoticesSent)
                .HasForeignKey(n => n.FromUser)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PaidPost>()
                .HasOne(n => n.User)
                .WithMany(u => u.PaidPosts)
                .HasForeignKey(n => n.IdUser)
                .OnDelete(DeleteBehavior.Restrict);     
            modelBuilder.Entity<PostComment>()
                .HasOne(n => n.User)
                .WithMany(u => u.PostComments)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<PostLike>()
                .HasOne(n => n.User)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PostView>()
                .HasOne(n => n.User)
                .WithMany(u => u.PostViews)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);  
            modelBuilder.Entity<PostSave>()
                .HasOne(n => n.Save)
                .WithMany(u => u.PostSaves)
                .HasForeignKey(n => n.IdSave)
                .OnDelete(DeleteBehavior.Restrict);  
            modelBuilder.Entity<ReplyResponse>()
                .HasOne(n => n.User)
                .WithMany(u => u.ReplyResponses)
                .HasForeignKey(n => n.IdUser)
                .OnDelete(DeleteBehavior.Restrict);   
            modelBuilder.Entity<Response>()
                .HasOne(n => n.User)
                .WithMany(u => u.Responses)
                .HasForeignKey(n => n.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flower>()
                .HasOne(n => n.User)
                .WithMany(u => u.Flowers)
                .HasForeignKey(n => n.IdUser)
                .OnDelete(DeleteBehavior.Restrict);


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}
