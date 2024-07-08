using Data.Enums;

namespace Data.DTO
{
    public class ExamTranfer
    {
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }
        public string Title { get; set; }

        public DateTime CreateDate { get; set; }

        public string Descripton { get; set; }

        public string Type { get; set; }

        public string Img { get; set; }

        public ModeExam Mode { get; set; }
    }
}
