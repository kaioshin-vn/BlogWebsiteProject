using BlogWebsite.Data;
using Data.Database.Table;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.TagController
{
    public class StatisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/getstatisUser/{year}")]
        public List<int> GetStatisUser(int year)
        {
            var listUser = _context.Users.Where(a => a.CreateTime.Value.Year == year).ToList();

            var listMonth = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var listTotalUser = new List<int>() ;

            foreach (var item in listMonth)
            {
                listTotalUser.Add(listUser.Where(a => a.CreateTime.Value.Month == item).Count());
            }          
            return listTotalUser;
        }

        [HttpGet("/getStatisTotal")]
        public Dictionary<string,string> getStatisTotal()
        {
            var total = new Dictionary<string,string>();
            total["Account"] = _context.Users.Count().ToString();
            total["Post"] = _context.Posts.Count(a => a.IsDeleted == false).ToString();
            total["Group"] = _context.Groups.Count(a => a.isDeleted == false).ToString();
            return total;
        }

        [HttpGet("/getstatisGroup/{year}")]
        public List<int> GetStatisGroup(int year)
        {
            var listUser = _context.Groups.Where(a => a.DateTime.Year == year && a.isDeleted == false).ToList();

            var listMonth = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var listTotalUser = new List<int>();

            foreach (var item in listMonth)
            {
                listTotalUser.Add(listUser.Where(a => a.DateTime.Month == item).Count());
            }
            return listTotalUser;
        }

        [HttpGet("/getstatisPost/{year}")]
        public List<int> GetStatisPost(int year)
        {
            var listUser = _context.Posts.Where(a => a.CreateDate.Value.Year == year && a.IsDeleted == false).ToList();

            var listMonth = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var listTotalUser = new List<int>();

            foreach (var item in listMonth)
            {
                listTotalUser.Add(listUser.Where(a => a.CreateDate.Value.Month == item).Count());
            }
            return listTotalUser;
        }

        [HttpGet("/getStatisticsByPeriod/{period}")]
        public async Task<IActionResult> GetStatisticsByPeriod(string period)
        {
            try
            {
                var now = DateTime.Now;
                var query = _context.Invoices
                    .Include(i => i.User)
                    .Include(i => i.RegistrationAdvertisement)
                        .ThenInclude(r => r.ServiceAdvertisementPricing)
                    .AsQueryable();

                // Filter by period
                switch (period.ToLower())
                {
                    case "day":
                        query = query.Where(i => i.PaymentDate.Date == now.Date);
                        break;
                    case "week":
                        var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                        query = query.Where(i => i.PaymentDate >= startOfWeek && i.PaymentDate <= now);
                        break;
                    case "month":
                        query = query.Where(i => i.PaymentDate.Month == now.Month && i.PaymentDate.Year == now.Year);
                        break;
                    case "year":
                        query = query.Where(i => i.PaymentDate.Year == now.Year);
                        break;
                }

                var invoices = await query.ToListAsync();

                var statistics = new
                {
                    Registrations = invoices
                        .Where(i => i.PaymentDate != DateTime.MinValue)
                        .Select(i => new Invoices
                        {
                            Id = i.Id,
                            User = i.User,
                            RegistrationAdvertisement = i.RegistrationAdvertisement,
                            Amount = i.Amount,
                            PaymentDate = i.PaymentDate,
                        })
                        .ToList(),

                    ChartData = new
                    {
                        TotalRevenue = invoices
                            .Where(i => i.PaymentDate != DateTime.MinValue)
                            .Sum(i => i.Amount),
                        TotalOrders = invoices.Count(),
                        PaidOrders = invoices.Count(i => i.PaymentDate != DateTime.MinValue),
                        UnpaidOrders = invoices.Count(i => i.PaymentDate == DateTime.MinValue)
                    }
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetStatisticsUser: {ex}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/getRevenueStatistics")]
        public async Task<IActionResult> GetRevenueStatistics()
        {
            try
            {
                var now = DateTime.Now;
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfYear = new DateTime(now.Year, 1, 1);

                var invoices = await _context.Invoices
                    .Include(i => i.RegistrationAdvertisement)
                    .Where(i => i.PaymentDate.Year == now.Year)
                    .ToListAsync();

                var statistics = new
                {
                    Today = new
                    {
                        TotalRevenue = invoices.Where(i => i.PaymentDate.Date == now.Date).Sum(i => i.Amount),
                        TotalOrders = invoices.Count(i => i.PaymentDate.Date == now.Date),
                        SuccessfulOrders = invoices.Count(i => i.PaymentDate.Date == now.Date && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Accept),
                        CancelledOrders = invoices.Count(i => i.PaymentDate.Date == now.Date && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Decline)
                    },
                    ThisWeek = new
                    {
                        TotalRevenue = invoices.Where(i => i.PaymentDate >= startOfWeek && i.PaymentDate <= now).Sum(i => i.Amount),
                        TotalOrders = invoices.Count(i => i.PaymentDate >= startOfWeek && i.PaymentDate <= now),
                        SuccessfulOrders = invoices.Count(i => i.PaymentDate >= startOfWeek && i.PaymentDate <= now && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Accept),
                        CancelledOrders = invoices.Count(i => i.PaymentDate >= startOfWeek && i.PaymentDate <= now && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Decline)
                    },
                    ThisMonth = new
                    {
                        TotalRevenue = invoices.Where(i => i.PaymentDate >= startOfMonth && i.PaymentDate <= now).Sum(i => i.Amount),
                        TotalOrders = invoices.Count(i => i.PaymentDate >= startOfMonth && i.PaymentDate <= now),
                        SuccessfulOrders = invoices.Count(i => i.PaymentDate >= startOfMonth && i.PaymentDate <= now && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Accept),
                        CancelledOrders = invoices.Count(i => i.PaymentDate >= startOfMonth && i.PaymentDate <= now && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Decline)
                    },
                    ThisYear = new
                    {
                        TotalRevenue = invoices.Where(i => i.PaymentDate >= startOfYear && i.PaymentDate <= now).Sum(i => i.Amount),
                        TotalOrders = invoices.Count(i => i.PaymentDate >= startOfYear && i.PaymentDate <= now),
                        SuccessfulOrders = invoices.Count(i => i.PaymentDate >= startOfYear && i.PaymentDate <= now && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Accept),
                        CancelledOrders = invoices.Count(i => i.PaymentDate >= startOfYear && i.PaymentDate <= now && i.RegistrationAdvertisement.State == Data.Enums.WaitState.Decline)
                    }
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/getStatisticsByDateRange")]
        public async Task<IActionResult> GetStatisticsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var query = _context.Invoices
                    .Include(i => i.User)
                    .Include(i => i.RegistrationAdvertisement)
                        .ThenInclude(r => r.ServiceAdvertisementPricing)
                    .Where(i => i.PaymentDate >= startDate && i.PaymentDate <= endDate)
                    .AsQueryable();

                var invoices = await query.ToListAsync();

                var statistics = new
                {
                    Registrations = invoices
                        .Where(i => i.PaymentDate != DateTime.MinValue)
                        .Select(i => new Invoices
                        {
                            Id = i.Id,
                            User = i.User,
                            RegistrationAdvertisement = i.RegistrationAdvertisement,
                            Amount = i.Amount,
                            PaymentDate = i.PaymentDate,
                        })
                        .ToList(),

                    ChartData = new
                    {
                        TotalRevenue = invoices
                            .Where(i => i.PaymentDate != DateTime.MinValue)
                            .Sum(i => i.Amount),
                        TotalOrders = invoices.Count(),
                        PaidOrders = invoices.Count(i => i.PaymentDate != DateTime.MinValue),
                        UnpaidOrders = invoices.Count(i => i.PaymentDate == DateTime.MinValue)
                    }
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
