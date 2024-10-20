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
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? EditDate { get; set; } // lưu lại lịch sử sửa
        public string? Content { get; set; }
        // Đường dẫn tới ảnh
        public string? ImgFile { get; set; }
        public string? VideoFile { get; set; }
        public string? Like { get; set; }
        public int View { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }
        public ICollection<PostTag>? TagPosts { get; set; }
        public ICollection<PostSave>? PostSaves { get; set; }
        public ICollection<PaidPost>? PaidPosts { get; set; }
        public ICollection<GroupPost>? GroupPost { get; set; }
        public ICollection<PostView>? PostViews { get; set; }
    }
}
