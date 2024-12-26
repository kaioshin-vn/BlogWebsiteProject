using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/getAllSvAds")]
        public async Task<IActionResult> GetAllMember()
        {
            var lstMember = await _context.ServiceAdvertisementPricing.OrderBy(a => a.Price).ToListAsync();
            return Ok(lstMember);
        }

        [HttpGet("/getSeviceAdsInfo/{Id}")]
        public async Task<IActionResult> getAdsInfo(Guid Id)
        {
            var lstMember = await _context.ServiceAdvertisementPricing.FirstOrDefaultAsync(a => a.Id == Id);
            return Ok(lstMember);
        }

        [HttpGet("/getAllRegisWaitBill/{Id}")]
        public async Task<IActionResult> GetWaitBill(Guid Id)
        {
            var lstMember =  _context.RegistrationAdvertisements.Where(a => a.IdUser == Id && a.State == Data.Enums.WaitState.Pending).ToList();
            return Ok(lstMember);
        }

        [HttpPost("/addNewAds")]
        public async Task AddNewAds([FromBody] RegistrationAdvertisement ads)
        {
            await _context.RegistrationAdvertisements.AddAsync(ads);
            _context.SaveChanges();
        }

        [HttpGet("/getRegisSeviceAds/{Id}")]
        public async Task<IActionResult> GetRegisAdsInfo(Guid Id)
        {
            var lstMember = await _context.RegistrationAdvertisements.FirstOrDefaultAsync(a => a.Id == Id);
            return Ok(lstMember);
        }

        [HttpGet("/getAllBillToPay/{Id}")]
        public async Task<IActionResult> GetAllBill(Guid Id)
        {
            var lstMember = _context.Invoices.Include(a => a.RegistrationAdvertisement).Where(a => a.IdUser == Id && a.PaymentDate == DateTime.MinValue).OrderByDescending(a => a.RegistrationAdvertisement.TimeAccept);
            return Ok(lstMember);
        }

        [HttpPost("/updteAdsRegis")]
        public async Task UpdateAds([FromBody] RegistrationAdvertisement ads)
        {
            _context.RegistrationAdvertisements.Update(ads);
            await _context.SaveChangesAsync();
        }

        [HttpDelete("/deleteRegisAds/{Id}")]
        public async Task DeleteAds(Guid Id)
        {
            var ads = await _context.RegistrationAdvertisements.FirstOrDefaultAsync(a => a.Id == Id);
            _context.RegistrationAdvertisements.Remove(ads);
            await _context.SaveChangesAsync();
        }

        [HttpGet("/getAllAdsRunning/{Id}")]
        public async Task<IActionResult> GetAdsRunning(Guid Id)
        {
            var lstMember = _context.RegistrationAdvertisements.Include(a => a.User).Include(a => a.Invoices).Where(a => a.IdUser == Id && a.State == Data.Enums.WaitState.Accept && a.Invoices.All(a => a.PaymentDate != DateTime.MinValue)).ToList();
            return Ok(lstMember);
        }

        [HttpGet("/renew/{IdSvAds}/{IdRegis}/{IdBill}")]
        public async Task RenewAds(Guid IdSvAds, Guid IdRegis, Guid IdBill )
        {
            var regis = await _context.RegistrationAdvertisements.FirstOrDefaultAsync(a => a.Id == IdRegis);
            var SvAds = await _context.RegistrationAdvertisements.FirstOrDefaultAsync(a => a.Id == IdSvAds);
            if (regis.TimeStart.AddDays(regis.DurationDays) > DateTime.Now)
            {
                var resttime = DateTime.Now - regis.TimeStart.AddDays(regis.DurationDays);
                regis.DurationDays =(resttime.Days + SvAds.DurationDays);
            }
            else
            {
                regis.DurationDays = SvAds.DurationDays;
                regis.TimeStart = DateTime.Now;
            }
        }
    }
}
