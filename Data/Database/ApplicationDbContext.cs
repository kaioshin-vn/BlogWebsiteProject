﻿using Data.Database.Table;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BlogWebsite.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {

        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<PostHideByRestricted> PostHideByRestricted { get; set; }
        public DbSet<ServiceAdvertisementPricing> ServiceAdvertisementPricing { get; set; }
        public DbSet<RegistrationAdvertisement> RegistrationAdvertisements { get; set; }


        public DbSet<PostHide> PostHides { get; set; }
        public DbSet<Messenger> Messengers { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<Petition> Petitions { get; set; }
        public DbSet<Save> Saves { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<RestrictedWord> RestrictedWords { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostSave> PostSaves { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ReplyResponse> ReplyResponses { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupPost> GroupPosts { get; set; }
        public DbSet<GroupTopic> GroupTopics { get; set; }
        public DbSet<MemberGroup> MemberGroups { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Flower> Flower { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.;Database=DATN;Trusted_Connection=True; TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostHide>().HasKey(o => new
            {
                o.IdPost,
                o.IdUser
            });

            modelBuilder.Entity<Flower>().HasKey(o => new 
            { 
                o.IdUser,
                o.IdFlower 
            });

            modelBuilder.Entity<RestrictedWord>().HasKey(o => new
            {
                o.Id,
            });

            modelBuilder.Entity<GroupPost>().HasKey(u => new
            {
                u.IdGroup,
                u.IdPost
            });

            modelBuilder.Entity<GroupTopic>().HasKey(u => new
            {
                u.IdGroup,
                u.IdTopic
            });

            modelBuilder.Entity<MemberGroup>().HasKey(u => new
            {
                u.IdGroup,
                u.IdMember
			});

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

            modelBuilder.Entity<SearchHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Keyword).IsRequired();
                entity.Property(e => e.SearchDate).IsRequired();
                entity.HasOne(n => n.ApplicationUser)
                .WithMany(u => u.SearchHistories)
                .HasForeignKey(n => n.IdUser);
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
