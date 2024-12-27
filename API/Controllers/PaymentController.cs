using BlogWebsite.Data;
using Data.Database.Table;
using Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("/getNextIdPay")]
        public async Task<IActionResult> GetAllMember()
        {
            var connection = _context.Database.GetDbConnection();
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT NEXT VALUE FOR MySequence;";
                var obj = cmd.ExecuteScalar();
                int anInt = Int32.Parse(obj.ToString());
                return Ok(anInt);
            }
        }

        [HttpGet("/invoicespaid/{Id}")]
        public async Task InvoicesPaid(Guid Id)
        {
            var iv = _context.Invoices.Include(a => a.RegistrationAdvertisement).FirstOrDefault(a => a.Id == Id);
            if (iv != null)
            {
                if (iv.PaymentDate == DateTime.MinValue)
                {
                    iv.PaymentDate = DateTime.Now;
                    _context.Invoices.Update(iv);

                    var regis = iv.RegistrationAdvertisement;
                    regis.TimeStart = DateTime.Now;
                    _context.RegistrationAdvertisements.Update(regis);

                    _context.SaveChanges();
                }
                Ok();
            }
            else
            {
                NotFound();
            }
        }

        [HttpGet("/getinvoice/{Id}")]
        public async Task<Invoices> GetInvoice(Guid Id)
        {
            var iv = _context.Invoices.Include(a => a.User).Include(a => a.RegistrationAdvertisement).FirstOrDefault(a => a.Id == Id);
            if (iv != null)
            {
                return iv;
            }
            else
            {
                return null;
            }
        }

        [HttpGet("/getListBillUser/{Id}")]
        public async Task<IActionResult> GetListInvoice(Guid Id)
        {
            var iv = _context.Invoices.Include(a => a.User).Include(a => a.RegistrationAdvertisement).Where(a => a.PaymentDate != DateTime.MinValue).ToList();
            
            return Ok(iv);
        }

        [HttpGet("/changeStateRegis/{Id}/{State}")]
        public async Task<IActionResult> ChangeState(Guid Id, WaitState State)
        {
            var regis = _context.RegistrationAdvertisements.FirstOrDefault(a => a.Id == Id);
            regis.State = State;
            if (State == WaitState.Accept)
            {
                regis.TimeAccept = DateTime.Now;
                var iv = new Invoices();
                iv.Id = Guid.NewGuid();
                iv.PaymentDate = DateTime.MinValue;
                iv.Amount = regis.Price;
                iv.IdUser = regis.IdUser;
                iv.IdRegis = regis.Id;
                _context.Invoices.Add(iv);
            }

            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("/checkwaitbill/{Id}")]
        public async Task<IActionResult> Check(Guid Id)
        {
            var iv = await _context.RegistrationAdvertisements.FirstOrDefaultAsync(a => a.Id == Id);

            var data = iv.State == Data.Enums.WaitState.Pending;

            return Ok(data);
        }
    }
}
