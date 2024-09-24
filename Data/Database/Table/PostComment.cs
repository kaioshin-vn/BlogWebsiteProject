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
    public class PostComment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("PostId")]
        public Post Post { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        // phản hồi bình luận

        [ForeignKey("ParentCommentId")]
        public Guid? ParentCommentId { get; set; } // ID bình luận cha (nếu có)
        public PostComment ParentComment { get; set; } // Tham chiếu đến bình luận cha
        public ICollection<PostComment> Replies { get; set; } // Danh sách các phản hồi
    }
}
