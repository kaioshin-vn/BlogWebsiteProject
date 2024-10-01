using System.ComponentModel;

namespace Client.Components.Account.Settings
{
    public class UserSettingViewModel
    {
        public Guid Id { get; set; }
        public string Descript { get; set; }
        public string Email { get; set; }   
        public string Img { get; set; } 
        public string PhoneNumber { get; set; }
    }
}
