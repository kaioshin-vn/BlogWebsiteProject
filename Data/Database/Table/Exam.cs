using BlogWebsite.Data;
using Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Tables
{
    public class Exam
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdUser { get; set; } 
        public string Title { get; set; }

        public DateTime CreateDate { get; set; }

        public string Descripton { get; set; }

        public string Type { get; set; }

        public string Img { get; set; }
        public int Time { get; set; }   

        public bool IsDeleted { get; set; } = false;

        public bool IsOpen { get; set; } = false;

        public string Code { get; set; } = "";


        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }

        public ICollection<ExamHistory>? ExamHistories { get; set; }
        public ICollection<Question>? Questions { get; set; }
    }
}
