﻿// <auto-generated />
using System;
using BlogWebsite.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BlogWebsite.Data.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("AnotherInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Data.Database.Table.Notice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("FromUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ToUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isRead")
                        .HasColumnType("bit");

                    b.Property<bool>("isSeen")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("FromUser");

                    b.HasIndex("ToUser");

                    b.ToTable("Notices");
                });

            modelBuilder.Entity("Data.Database.Table.PaidPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AmountPay")
                        .HasColumnType("int");

                    b.Property<Guid>("IdPost")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdPost");

                    b.HasIndex("IdUser");

                    b.ToTable("PaidPosts");
                });

            modelBuilder.Entity("Data.Database.Table.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Likes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NomalizedTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("View")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Data.Database.Table.PostSave", b =>
                {
                    b.Property<Guid>("IdSave")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdPost")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdSave", "IdPost");

                    b.HasIndex("IdPost");

                    b.ToTable("PostSaves");
                });

            modelBuilder.Entity("Data.Database.Table.PostTag", b =>
                {
                    b.Property<Guid>("IdTag")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdPost")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdTag", "IdPost");

                    b.HasIndex("IdPost");

                    b.ToTable("PostTags");
                });

            modelBuilder.Entity("Data.Database.Table.ReplyResponse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IdResponse")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Likes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("Mention")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("IdResponse");

                    b.HasIndex("IdUser");

                    b.ToTable("ReplyResponses");
                });

            modelBuilder.Entity("Data.Database.Table.Response", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("IdPost")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Likes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("Mention")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("IdPost");

                    b.HasIndex("IdUser");

                    b.ToTable("Responses");
                });

            modelBuilder.Entity("Data.Database.Table.Save", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SaveName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("Saves");
                });

            modelBuilder.Entity("Data.Database.Table.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("TagName")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Data.Tables.Exam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descripton")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOpen")
                        .HasColumnType("bit");

                    b.Property<int>("Time")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("Exams");
                });

            modelBuilder.Entity("Data.Tables.ExamHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdExam")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Info")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Scores")
                        .HasColumnType("float");

                    b.Property<DateTime>("TimeEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TimeStart")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("IdExam");

                    b.HasIndex("IdUser");

                    b.ToTable("ExamHistories");
                });

            modelBuilder.Entity("Data.Tables.ExamHistoryDetails", b =>
                {
                    b.Property<Guid>("IdExamHistoryDetail")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Answer1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IdEH")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumbersChose")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumbersRight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isMultiple")
                        .HasColumnType("bit");

                    b.HasKey("IdExamHistoryDetail");

                    b.HasIndex("IdEH");

                    b.ToTable("ExamHistoryDetails");
                });

            modelBuilder.Entity("Data.Tables.Question", b =>
                {
                    b.Property<Guid>("IdQuestion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Answer1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer4")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Answer5")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentQuestion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IdExam")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumberRight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isMultiple")
                        .HasColumnType("bit");

                    b.HasKey("IdQuestion");

                    b.HasIndex("IdExam");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens", (string)null);
                });

            modelBuilder.Entity("Data.Database.Table.Notice", b =>
                {
                    b.HasOne("BlogWebsite.Data.ApplicationUser", "UserSend")
                        .WithMany()
                        .HasForeignKey("FromUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlogWebsite.Data.ApplicationUser", "UserReceive")
                        .WithMany()
                        .HasForeignKey("ToUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserReceive");

                    b.Navigation("UserSend");
                });

            modelBuilder.Entity("Data.Database.Table.PaidPost", b =>
                {
                    b.HasOne("Data.Database.Table.Post", "Post")
                        .WithMany("PaidPosts")
                        .HasForeignKey("IdPost")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlogWebsite.Data.ApplicationUser", "User")
                        .WithMany("PaidPosts")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Database.Table.Post", b =>
                {
                    b.HasOne("BlogWebsite.Data.ApplicationUser", "User")
                        .WithMany("Post")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Database.Table.PostSave", b =>
                {
                    b.HasOne("Data.Database.Table.Post", "Post")
                        .WithMany("PostSaves")
                        .HasForeignKey("IdPost")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Database.Table.Save", "Save")
                        .WithMany("PostSaves")
                        .HasForeignKey("IdSave")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("Save");
                });

            modelBuilder.Entity("Data.Database.Table.PostTag", b =>
                {
                    b.HasOne("Data.Database.Table.Post", "Post")
                        .WithMany("TagPosts")
                        .HasForeignKey("IdPost")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Database.Table.Tag", "Tag")
                        .WithMany("TagPosts")
                        .HasForeignKey("IdTag")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Data.Database.Table.ReplyResponse", b =>
                {
                    b.HasOne("Data.Database.Table.Response", "Response")
                        .WithMany()
                        .HasForeignKey("IdResponse")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlogWebsite.Data.ApplicationUser", "User")
                        .WithMany("ReplyResponses")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Response");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Database.Table.Response", b =>
                {
                    b.HasOne("Data.Database.Table.Post", "Post")
                        .WithMany()
                        .HasForeignKey("IdPost")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlogWebsite.Data.ApplicationUser", "User")
                        .WithMany("Responses")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Database.Table.Save", b =>
                {
                    b.HasOne("BlogWebsite.Data.ApplicationUser", "User")
                        .WithMany("Saves")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Tables.Exam", b =>
                {
                    b.HasOne("BlogWebsite.Data.ApplicationUser", "User")
                        .WithMany("Exams")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Tables.ExamHistory", b =>
                {
                    b.HasOne("Data.Tables.Exam", "Exam")
                        .WithMany("ExamHistories")
                        .HasForeignKey("IdExam")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlogWebsite.Data.ApplicationUser", "User")
                        .WithMany("ExamHistories")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Tables.ExamHistoryDetails", b =>
                {
                    b.HasOne("Data.Tables.ExamHistory", "ExamHistory")
                        .WithMany("ExamHistoryDetails")
                        .HasForeignKey("IdEH")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExamHistory");
                });

            modelBuilder.Entity("Data.Tables.Question", b =>
                {
                    b.HasOne("Data.Tables.Exam", "Exam")
                        .WithMany("Questions")
                        .HasForeignKey("IdExam")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlogWebsite.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("BlogWebsite.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BlogWebsite.Data.ApplicationUser", b =>
                {
                    b.Navigation("ExamHistories");

                    b.Navigation("Exams");

                    b.Navigation("PaidPosts");

                    b.Navigation("Post");

                    b.Navigation("ReplyResponses");

                    b.Navigation("Responses");

                    b.Navigation("Saves");
                });

            modelBuilder.Entity("Data.Database.Table.Post", b =>
                {
                    b.Navigation("PaidPosts");

                    b.Navigation("PostSaves");

                    b.Navigation("TagPosts");
                });

            modelBuilder.Entity("Data.Database.Table.Save", b =>
                {
                    b.Navigation("PostSaves");
                });

            modelBuilder.Entity("Data.Database.Table.Tag", b =>
                {
                    b.Navigation("TagPosts");
                });

            modelBuilder.Entity("Data.Tables.Exam", b =>
                {
                    b.Navigation("ExamHistories");

                    b.Navigation("Questions");
                });

            modelBuilder.Entity("Data.Tables.ExamHistory", b =>
                {
                    b.Navigation("ExamHistoryDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
