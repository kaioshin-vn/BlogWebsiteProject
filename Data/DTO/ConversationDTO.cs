using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class ConversationDTO
    {
        public Guid Id { get; set; }

        public string? Avatar { get; set; }
        public string? FullName { get; set; }
        public string LastestMes { get; set; }
        public string Time { get; set; }

        public bool isRead { get; set; }    
        public bool isAdminRead { get; set; }
        public DateTime LastTime { get; set; }
    }
}
