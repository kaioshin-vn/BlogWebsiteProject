using BlogWebsite.Data;
using Data.Database.Table;
using Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    public class ComunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ComunitiesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("/getListComunities/{page}/{Search?}")]
        public async Task<List<ComunitiesDTO>> GetComunitiesDTO(int page, string? Search)
        {
            page = page - 1;

            var listGroup = new List<Group>();

            if (Search != null)
            {
                listGroup = _context.Groups.Include(a => a.MemberGroups).Where(a => a.isDeleted == false && a.Name.ToLower().Contains(Search.ToLower())).Skip(page * 10).Take(10).ToList();
            }
            else
            {
                listGroup = _context.Groups.Include(a => a.MemberGroups).Where(a => a.isDeleted == false).Skip(page * 10).Take(10).ToList();
            }

            var listcomunities = new List<ComunitiesDTO>();

            foreach (var group in listGroup)
            {
                var comu = new ComunitiesDTO();
                comu.Id = group.IdGroup;
                comu.Name = group.Name;
                comu.MemberCount = group.MemberGroups.Count;
                var listidpost = new List<Guid>();
                listidpost = _context.GroupPosts.Include(a => a.Post).Where(a => a.IdGroup == group.IdGroup && a.Post.IsDeleted == false).Select(a => a.IdPost).ToList();

                comu.PostCount = listidpost.Count;

                comu.ViolenceCount = _context.Reports.Where(a => listidpost.Contains(a.IdPost)).Count();

                listcomunities.Add(comu);
            }

            return listcomunities;

        }

        [HttpGet("/GetTotalComunities/{Search?}")]
        public async Task<int> GetTotalComunitiesDTO( string? Search)
        {
            if (Search != null)
            {
                return _context.Groups.Include(a => a.MemberGroups).Where(a => a.isDeleted == false && a.Name.ToLower().Contains(Search.ToLower())).Count() / 10;
            }
            else
            {
                return _context.Groups.Include(a => a.MemberGroups).Where(a => a.isDeleted == false ).Count() / 10;
            }
        }

        [HttpGet("/GetIdAdminComunities/{Id}")]
        public async Task<Guid> GetTotalComunitiesDTO(Guid Id)
        {
            return _context.MemberGroups.FirstOrDefaultAsync(a => a.IdGroup == Id && a.Position == Data.Enums.Position.Chief).Result.IdMember;
        }
    }
}
