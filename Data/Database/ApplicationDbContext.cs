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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.;Database=DATN;Trusted_Connection=True; TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<IdentityUserLogin<Guid>>();
            modelBuilder.Ignore<IdentityUserClaim<Guid>>();
            modelBuilder.Ignore<IdentityRoleClaim<Guid>>();

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
