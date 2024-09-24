using BlogWebsite.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database.Table
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }
        public string Title { get; set; }
        public string NomalizedTitle { get; set; }

        public DateTime CreateDate { get; set; }

        public string Content { get; set; }

        public int Like { get; set; }

        public int View { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
        public ICollection<PostTag>? TagPosts { get; set; }
        public ICollection<PostSave>? PostSaves { get; set; }
        public ICollection<PaidPost>? PaidPosts { get; set; }


        // Đường dẫn tới ảnh
        public string? ImagePath { get; set; }

        public ICollection<PostComment> PostComments { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostView> PostViews { get; set; }
    }
}
