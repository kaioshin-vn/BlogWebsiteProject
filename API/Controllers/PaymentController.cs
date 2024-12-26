using BlogWebsite.Data;
using Data.Database.Table;
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
    }
}
