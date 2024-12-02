
using BlogWebsite.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ASM_PH35423.Controllers
{
    [ApiController]

    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;
        RoleManager<IdentityRole<Guid>> _roleManage;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        public AuthorController(ApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManage , UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _roleManage = roleManage;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPatch("/UpdateRole")]
        public void UpdateUserRole(IdentityRole<Guid> Role)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id == Role.Id);
            role.Name = Role.Name;
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        [HttpGet("/GetListRole")]
        public List<IdentityRole<Guid>> GetAllRole()
        {
            var listRole = new List<IdentityRole<Guid>>();

            listRole = _context.Roles.ToList();
            return listRole;
        }

        [HttpPost("/AddRole")]
        public async Task AddRole([FromBody] string Name)
        {
            var role = new IdentityRole<Guid>(Name);
            await _roleManage.CreateAsync(role);
        }

        [HttpPost("/CheckName")]
        public async void CheckRoleName(string Name)
        {
            var role = new IdentityRole<Guid>(Name);
            await _roleManage.CreateAsync(role);
            _context.SaveChanges();
        }

        [HttpDelete("/DeleteRole/{Id}")]
        public void DeleteRole(Guid Id)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id == Id);
            _context.Roles.Remove(role);
            _context.SaveChanges();
        }

        [HttpGet("/GetListUser")]
        public List<ApplicationUser> GetAllUser()
        {
            return _context.Users.ToList();
        }

        [HttpGet("/GetRoleUser/{Id}")]
        public List<string?> GetRoleUser(Guid Id)
        {
            var listRoleUser = new List<string?>();
            var userRoles = _context.UserRoles.Where(a => a.UserId ==  Id);
            listRoleUser =  _context.Roles.Join(userRoles, a => a.Id, b => b.RoleId, (c, d) => c.Name).ToList();
            return listRoleUser;
        }

        [HttpGet("/GetIdRoleUser/{Id}")]
        public List<Guid> GetIdRoleUser(Guid Id)
        {
            var listRoleUser = new List<Guid>();
            listRoleUser = _context.UserRoles.Where(a => a.UserId == Id).Select(a => a.RoleId).ToList();
            return listRoleUser;
        }

        [HttpPost("/RegisTeacher")]
        public async Task RegisTeacher([FromBody] Guid IdUser)
        {
            var user = await _userManager.FindByIdAsync(IdUser.ToString());
            await _userManager.AddToRoleAsync(user, "Teacher");
        }


        [HttpPost("/UpdateUser/{IdUser}")]
        public void UpdateUserRole([FromBody] List<Guid> ListRole, Guid IdUser)
        {
            if (ListRole.Count == 0)
            {
                var listRoleUserDelete = _context.UserRoles.Where(a => a.UserId == IdUser).ToList();
                if (listRoleUserDelete.Count != 0)
                {
                    foreach (var item in listRoleUserDelete)
                    {
                        _context.UserRoles.Remove(item);
                        _context.SaveChanges();
                    }
                }
                return;
            }

            var listRoleUser = _context.UserRoles.Where(a => a.UserId == IdUser).ToList();

            var listDelete = new List<Guid>();
            var listAdd = ListRole;

            foreach (var item in listRoleUser)
            {
                if (ListRole.Any(a => a == item.RoleId))
                {
                    listAdd.Remove(item.RoleId);
                }
                else
                {
                    listDelete.Add(item.RoleId);
                }
            }

            foreach (var item in listDelete)
            {
                var role = listRoleUser.FirstOrDefault(a => a.RoleId == item);
                _context.UserRoles.Remove(role);
            }
            _context.SaveChanges();

            foreach (var item in listAdd)
            {
                var newRoleUser = new IdentityUserRole<Guid>();
                newRoleUser.RoleId = item;
                newRoleUser.UserId = IdUser;
                _context.UserRoles.Add(newRoleUser);
            }
            _context.SaveChanges();
        }

        [HttpGet("/GetUser/{Id}")]
        public ApplicationUser GetUser(Guid Id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == Id);
            return user;
        }

        [HttpGet("/getIdUserRole")]
        public async Task<Guid> getIdUserRole()
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "Người dùng");
            if (role != null)
            {
                return role.Id;
            }
            return Guid.Empty;
        }
    }
}
