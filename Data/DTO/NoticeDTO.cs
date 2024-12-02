using Data.Enums;

namespace Data.DTO
{
    public class NoticeDTO
    {
        public Guid Id { get; set; }

        public Guid IdUserSend { get; set; }
        
        public string UserNameSend { get; set; }
        public string AvatarUserSend { get; set; }

        public string Link { get; set; }

        public string Content { get; set; }

        public string Time { get; set; }

        public bool isRead { get; set; }

    }
}
