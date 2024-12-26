using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.BookingBanner
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceAdvertisementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ServiceAdvertisementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAllService")]
        public async Task<IActionResult> GetAllService()
        {
            return Ok(await _context.ServiceAdvertisementPricing.Where(x => x.IsDelete == false).ToListAsync());
        }

        [HttpGet("getByIdService/{id}")]
        public async Task<IActionResult> GetByIdService(Guid id)
        {
            return Ok(await _context.ServiceAdvertisementPricing.FindAsync(id));
        }

        [HttpPost("addService")]
        public async Task<IActionResult> AddService(ServiceAdvertisementPricing service)
        {
            var newService = new ServiceAdvertisementPricing()
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                DurationDays = service.DurationDays,
            };
            _context.ServiceAdvertisementPricing.Add(newService);
            await _context.SaveChangesAsync();
            return Ok(newService);
        }

        [HttpPut("updateService/{id}")]
        public async Task<IActionResult> UpdateService(Guid id, ServiceAdvertisementPricing service)
        {
            var getById = await _context.ServiceAdvertisementPricing.FindAsync(id);
            if (getById == null) { return BadRequest(); }

            getById.Name = service.Name;
            getById.Description = service.Description;
            getById.Price = service.Price;
            getById.DurationDays = service.DurationDays;

            _context.ServiceAdvertisementPricing.Update(getById);
            await _context.SaveChangesAsync();
            return Ok(getById);
        }

        [HttpPut("deleteService/{id}")]
        public async Task<IActionResult>DeleteService(Guid id)
        {
            var getById = await _context.ServiceAdvertisementPricing.FindAsync(id);
            if (getById == null) { return BadRequest(); }

            getById.IsDelete = true;

            _context.ServiceAdvertisementPricing.Update(getById);
            await _context.SaveChangesAsync();
            return Ok(getById);
        }

    }
}
