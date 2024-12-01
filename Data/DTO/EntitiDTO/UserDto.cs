namespace Data.DTO.EntitiDTO
{
    public class UserDto
    {
        public string? Descript { get; set; }
        public string? Img { get; set; }
        public string? FullName { get; set; }
        public string? ImgCover { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Id { get; set; }
        public string? IdViewer { get; set; }
        public int CountFollow { get; set; }
        public bool IsFollowing { get; set; }
    }
}
