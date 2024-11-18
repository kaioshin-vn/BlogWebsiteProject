using Data.Database.Table;
using Data.Tables;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebsite.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Descript { get; set; }
        public string? Img { get; set; }
        public string? FullName { get; set; }
        public string? ImgCover { get; set; }
        public string? Address { get; set; }
        public ICollection<Exam>? Exams { get; set; }
        public ICollection<Flower>? Flowers { get; set; }

        public ICollection<ExamHistory>? ExamHistories { get; set; }
        public ICollection<Post>? Post { get; set; }
        public ICollection<Save>? Saves { get; set; }
        public ICollection<PaidPost>? PaidPosts { get; set; }
        public ICollection<Response>? Responses { get; set; }
        public ICollection<ReplyResponse>? ReplyResponses { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
        public ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
        public ICollection<PostView> PostViews { get; set; }
        public ICollection<Notice> NoticesSent { get; set; }
        public ICollection<Notice> NoticesReceived { get; set; }
        public ICollection<MemberGroup> MemberGroups { get; set; }
    }
}
