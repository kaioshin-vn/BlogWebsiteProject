using Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Tables
{
    public class Question
    {
        [Key]
        public Guid IdQuestion { get; set; }

        public Guid IdExam { get; set; }

        public string ContentQuestion { get;set; }

        public string Img { get; set; }

        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string Answer5 { get; set; }

        public bool isMultiple { get; set; }
        public string NumberRight { get; set; }


        [ForeignKey("IdExam")]
        public Exam? Exam { get; set; }
    }
}
