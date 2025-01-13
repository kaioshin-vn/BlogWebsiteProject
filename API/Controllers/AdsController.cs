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
            var lstMember = await _context.ServiceAdvertisementPricing.OrderBy(a => a.Price).Where(a => a.IsDelete == false).ToListAsync();
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
            var lstMember =  _context.RegistrationAdvertisements.Include(a => a.ServiceAdvertisementPricing).Where(a => a.IdUser == Id && a.ServiceAdvertisementPricing.IsDelete == false && a.State == Data.Enums.WaitState.Pending).ToList();
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
            var lstMember = _context.Invoices.Include(a => a.RegistrationAdvertisement).ThenInclude(a => a.ServiceAdvertisementPricing).Where(a => a.IdUser == Id && a.RegistrationAdvertisement.ServiceAdvertisementPricing.IsDelete == false && a.PaymentDate == DateTime.MinValue).OrderByDescending(a => a.RegistrationAdvertisement.TimeAccept);
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

        [HttpGet("/getAdsRunningSys")]
        public async Task<IActionResult> GetAdsRunningSys()
        {
            var lstMember = _context.RegistrationAdvertisements.Include(a => a.User).Include(a => a.Invoices).Where(a => a.State == Data.Enums.WaitState.Accept && a.TimeStart.AddDays(a.DurationDays) >= DateTime.Now && a.Invoices.All(a => a.PaymentDate != DateTime.MinValue)).ToList();
            return Ok(lstMember);
        }

        [HttpGet("/stopAds/{Id}")]
        public async Task<IActionResult> StopAds(Guid Id)
        {
            var ads = _context.RegistrationAdvertisements.FirstOrDefault(a => a.Id == Id);
            ads.State = Data.Enums.WaitState.Decline;
            _context.RegistrationAdvertisements.Update(ads);
            _context.SaveChanges();
            return Ok(ads);
        }

        [HttpGet("/renew/{IdSvAds}/{IdRegis}/{IdBill}/{IdUser}")]
        public async Task RenewAds(Guid IdSvAds, Guid IdRegis, Guid IdBill, Guid IdUser)
        {
            var regis = await _context.RegistrationAdvertisements.FirstOrDefaultAsync(a => a.Id == IdRegis);
            var SvAds = await _context.ServiceAdvertisementPricing.FirstOrDefaultAsync(a => a.Id == IdSvAds);
            if (regis.TimeStart.AddDays(regis.DurationDays) > DateTime.Now)
            {
                regis.DurationDays += SvAds.DurationDays;
            }
            else
            {
                regis.DurationDays = SvAds.DurationDays;
                regis.TimeStart = DateTime.Now;
            }

            _context.RegistrationAdvertisements.Update(regis);

            var bill = await _context.Invoices.FirstOrDefaultAsync(a => a.Id == IdBill);

            if (bill == null)
            {
                bill = new Invoices();
                bill.Id = IdBill;
                bill.PaymentDate = DateTime.Now;
                bill.Amount = SvAds.Price;
                bill.IdRegis = IdRegis;
                bill.IdUser = IdUser;

                _context.Invoices.Add(bill);
            }

            _context.SaveChanges();
        }


        [HttpGet("/getRegisWaitUser")]
        public async Task<IActionResult> GetRegisWaite()
        {
            var lstMember = _context.RegistrationAdvertisements.Include(a => a.User).Include(a => a.ServiceAdvertisementPricing).Where( a => a.ServiceAdvertisementPricing.IsDelete == false && a.State == Data.Enums.WaitState.Pending).ToList();
            return Ok(lstMember);
        }

        [HttpGet("/getcurrentads")]
        public async Task<IActionResult> GetCurrentAds()
        {
            var lstMember = _context.RegistrationAdvertisements.Include(a => a.User).Include(a => a.Invoices).Where(a => a.State == Data.Enums.WaitState.Accept && a.Invoices.All(a => a.PaymentDate != DateTime.MinValue) && a.TimeStart.AddDays(a.DurationDays) > DateTime.Now).OrderByDescending(a => a.Id).ToList();

            var listId = lstMember.Select(a => a.Id).ToList();

            if (lstMember.Count != 0)
            {
                if (StaticVariable.IdAdsCurrent == Guid.Empty)
                {
                    StaticVariable.IdAdsCurrent = lstMember[0].Id;
                    return Ok(lstMember[0]);
                }
                else
                {
                    var idAds = GetNextElement(listId, StaticVariable.IdAdsCurrent);
                    if (idAds == Guid.Empty)
                    {
                        return Ok(lstMember[0]);
                    }
                    else
                    {
                        var ads = lstMember.FirstOrDefault(a => a.Id == idAds);
                        StaticVariable.IdAdsCurrent = idAds;
                        return Ok(ads);
                    }
                }
            }
            else
            {
                return Ok(null);
            }
        }

        private static Guid GetNextElement(List<Guid> list, Guid id)
        {
            int index = list.IndexOf(id);

            if (index != -1)
            {
                // Nếu là phần tử cuối cùng, trả về phần tử đầu tiên
                if (index == list.Count - 1)
                {
                    return list[0];
                }
                // Ngược lại, trả về phần tử tiếp theo
                return list[index + 1];
            }

            return Guid.Empty;
        }
    }
}
