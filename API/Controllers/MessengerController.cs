using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class MessengerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MessengerController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("/getallconversation/{Id}")]
        public async Task<List<ConversationDTO>> GetAllConver(Guid Id)
        {
            var listconverDTO = new List<ConversationDTO>();

            var listConver = _context.Conversations.Include(a => a.UserReceive).Where(a => a.IdUser == Id).ToList();

            foreach (var item in listConver)
            {
                var cvDTO = new ConversationDTO();
                cvDTO.Id = item.Id;

                cvDTO.FullName = item.UserReceive == null ? null : item.UserReceive.FullName;
                cvDTO.Avatar = item.UserReceive == null ? null : item.UserReceive.Img;

                var lastMes = await _context.Messengers.OrderByDescending(a => a.CreateTime).FirstOrDefaultAsync(a => a.IdConversation == item.Id);
                if (lastMes.IdUserSend == Id)
                {
                    cvDTO.LastestMes = $"Bạn: {lastMes.Content}";
                }
                else
                {
                    cvDTO.LastestMes = lastMes.Content;
                }
                cvDTO.Time = ProcessTime(lastMes.CreateTime);
                cvDTO.LastTime = lastMes.CreateTime;
                cvDTO.isRead = item.isRead;
                listconverDTO.Add(cvDTO);
            }

            listconverDTO = listconverDTO.OrderByDescending(a => a.LastTime).ToList();
            return listconverDTO;
        }

        [HttpGet("/getalladminaccountconversation/{Id}")]
        public async Task<List<ConversationDTO>> GetAllAdminAccountConver(Guid Id)
        {
            var listconverDTO = new List<ConversationDTO>();

            var listConver = _context.Conversations.Include(a => a.UserSend).Where(a => a.IdUserReceive == Id).ToList();

            foreach (var item in listConver)
            {
                var cvDTO = new ConversationDTO();
                cvDTO.Id = item.Id;

                cvDTO.FullName = item.UserSend == null ? null : item.UserSend.FullName;
                cvDTO.Avatar = item.UserSend == null ? null : item.UserSend.Img;

                var lastMes = await _context.Messengers.OrderByDescending(a => a.CreateTime).FirstOrDefaultAsync(a => a.IdConversation == item.Id);
                if (lastMes.IdUserSend == Id)
                {
                    cvDTO.LastestMes = $"Bạn: {lastMes.Content}";
                }
                else
                {
                    cvDTO.LastestMes = lastMes.Content;
                }
                cvDTO.Time = ProcessTime(lastMes.CreateTime);
                cvDTO.LastTime = lastMes.CreateTime;
                cvDTO.isRead = item.isAdminRead;
                listconverDTO.Add(cvDTO);
            }

            listconverDTO = listconverDTO.OrderByDescending(a => a.LastTime).ToList();
            return listconverDTO;
        }

        [HttpGet("/getallwaitconversation")]
        public async Task<List<ConversationDTO>> GetAllWaitConver()
        {
            var listconverDTO = new List<ConversationDTO>();

            var listConver = _context.Conversations.Include(a => a.UserSend).Where(a => a.IdUserReceive == null).ToList();

            foreach (var item in listConver)
            {
                var cvDTO = new ConversationDTO();
                cvDTO.Id = item.Id;

                cvDTO.FullName = item.UserSend == null ? null : item.UserSend.FullName;
                cvDTO.Avatar = item.UserSend == null ? null : item.UserSend.Img;

                var lastMes = await _context.Messengers.OrderByDescending(a => a.CreateTime).FirstOrDefaultAsync(a => a.IdConversation == item.Id);


                cvDTO.LastestMes = lastMes.Content;
                cvDTO.Time = ProcessTime(lastMes.CreateTime);
                cvDTO.LastTime = lastMes.CreateTime;
                cvDTO.isRead = item.isRead;
                listconverDTO.Add(cvDTO);
            }

            listconverDTO = listconverDTO.OrderByDescending(a => a.LastTime).ToList();
            return listconverDTO;
        }

        [HttpGet("/converReadedByUser/{Id}")]
        public async Task ReadedByUser(Guid Id)
        {
            var conver = _context.Conversations.FirstOrDefault(a => a.Id == Id);
            conver.isRead = true;
            _context.Conversations.Update(conver);
            await _context.SaveChangesAsync();
        }

        [HttpGet("/converUserSendMes/{Id}")]
        public async Task UserSendMes(Guid Id)
        {
            var conver = _context.Conversations.FirstOrDefault(a => a.Id == Id);
            conver.isRead = true;
            conver.isAdminRead = false;
            _context.Conversations.Update(conver);
            await _context.SaveChangesAsync();
        }

        [HttpGet("/converReadedByAdmin/{Id}")]
        public async Task ReadedByAdmin(Guid Id)
        {
            var conver = _context.Conversations.FirstOrDefault(a => a.Id == Id);
            conver.isAdminRead = true;
            _context.Conversations.Update(conver);
            await _context.SaveChangesAsync();
        }

        [HttpGet("/converAdminSendMes/{Id}")]
        public async Task AdminSendMes(Guid Id)
        {
            var conver = _context.Conversations.FirstOrDefault(a => a.Id == Id);
            conver.isAdminRead = true;
            conver.isRead = false;
            _context.Conversations.Update(conver);
            await _context.SaveChangesAsync();
        }

        [HttpGet("/getconver/{Id}")]
        public async Task<Conversation> GetConver(Guid Id)
        {
            var conver = _context.Conversations.FirstOrDefault(a => a.Id == Id);
            return conver;
        }

        private string ProcessTime(DateTime? CreateDate)
        {
            string Date;

            var timeCacul = DateTime.Now - CreateDate.Value;

            int yearsDifference = DateTime.Now.Year - CreateDate.Value.Year;
            if (DateTime.Now < CreateDate.Value.AddYears(yearsDifference))
            {
                yearsDifference--;
            }

            int monthsDifference = (DateTime.Now.Year - CreateDate.Value.Year) * 12 + DateTime.Now.Month - CreateDate.Value.Month;
            if (DateTime.Now.Day < CreateDate.Value.Day)
            {
                monthsDifference--;
            }

            if (yearsDifference >= 1)
            {
                Date = $"{yearsDifference} năm";
            }
            else if (monthsDifference >= 1)
            {
                Date = $"{monthsDifference} tháng";
            }
            else if (timeCacul.TotalDays >= 1)
            {
                Date = $"{(int)timeCacul.TotalDays} ngày";
            }
            else if (timeCacul.TotalHours >= 1)
            {
                Date = $"{(int)timeCacul.TotalHours} giờ";
            }
            else if (timeCacul.TotalMinutes >= 1)
            {
                Date = $"{(int)timeCacul.TotalMinutes} phút";
            }
            else
            {
                Date = "Vừa xong";
            }
            return Date;
        }

    }
}
