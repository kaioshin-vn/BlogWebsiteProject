using BlogWebsite.Data;
using Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Tables
{
    public class ExamHistory
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdExam { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string Info { get; set; }

        public double Scores { get; set; }

        [ForeignKey("IdUser")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("IdExam")]
        public Exam? Exam { get; set; }

        public ICollection<ExamHistoryDetails>? ExamHistoryDetails { get; set; }
    }
}
