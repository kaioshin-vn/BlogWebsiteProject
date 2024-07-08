namespace Data.DTO
{
    public class QuestionTransferCustomer
    {
        public Guid Id { get; set; }
        public string NumbersAnwser { get; set; }
    }

    public class QuestionTransferServer
    {
        public Guid Id { get; set; }

        public string Question { get; set; }

        public string? Img { get; set; }
        public bool IsMultiple { get; set; }

        public List<AnswerTransfer>? ListAnswer { get; set; } = new List<AnswerTransfer>();
    }

    public class AnswerTransfer
    {
        public int Value { get; set; }

        public string Answer { get; set; }
    }

    public class HistoryTransfer
    {

        public string Question { get; set; }

        public string? Img { get; set; }
        public bool IsMultiple { get; set; }

        public string Resolve { get; set; }
        public string AnwserCustom { get; set; }

        public List<AnswerTransfer>? ListAnswer { get; set; } = new List<AnswerTransfer>();
    }

}
