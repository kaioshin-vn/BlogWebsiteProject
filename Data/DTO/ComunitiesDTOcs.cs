using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class ComunitiesDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int MemberCount { get; set; }
        public int PostCount { get; set; }
        public int ViolenceCount { get; set; }
    }
}
