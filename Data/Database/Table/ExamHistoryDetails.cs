using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Tables
{
    public class ExamHistoryDetails
    {
        [Key]
        public Guid IdExamHistoryDetail { get; set; }

        public Guid IdEH { get; set; }

        public string Question { get; set; }

        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Img { get; set; }

        public bool isMultiple { get; set; }

        public string NumbersRight { get; set; }

        public string NumbersChose { get; set; }

        [ForeignKey("IdEH")]
        public ExamHistory ? ExamHistory { get; set; }

    }
}
