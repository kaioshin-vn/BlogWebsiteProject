using BlogWebsite.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilesController(ApplicationDbContext context)
        {
            _context = context;
        }
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        [HttpGet("DownloadExcel/{year}")]
        public IActionResult DownloadExcel(int year)
        {
            byte[] reportBytes;
            using (var package = Utils.Utils.createExcelPackage(_context, year))
            {
                reportBytes = package.GetAsByteArray();
            }
            return File(reportBytes, XlsxContentType, $"MyReport{DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}.xlsx");
        }
    }

}
