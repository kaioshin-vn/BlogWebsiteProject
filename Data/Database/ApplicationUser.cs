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
        public string? AnotherInfo { get; set; }    
        public ICollection<Exam>? Exams { get; set; }

        public ICollection<ExamHistory>? ExamHistories { get; set; }
        public ICollection<Post>? Post { get; set; }
        public ICollection<Save>? Saves { get; set; }
        public ICollection<PaidPost>? PaidPosts { get; set; }
        public ICollection<Response>? Responses { get; set; }
        public ICollection<ReplyResponse>? ReplyResponses { get; set; }
<<<<<<< HEAD
		public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
		public ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
	}
=======

        public ICollection<PostComment> PostComments { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostView> PostViews { get; set; }
        public ICollection<Notice> NoticesSent { get; set; }
        public ICollection<Notice> NoticesReceived { get; set; }
    }
>>>>>>> be11ec0564bf42a230404e5d88977f8170774da0

}
