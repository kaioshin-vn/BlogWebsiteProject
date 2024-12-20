//using BlogWebsite.Data;
//using Client.VNPayService;
//using Data.Database.Table;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace API.Controllers
//{
//	[Route("api/payment")]
//	[ApiController]
//	public class PaymentController : ControllerBase
//	{
//		private readonly VnPayService _vnPayService;
//		private readonly ApplicationDbContext _context;

//        public PaymentController(VnPayService vnPayService, ApplicationDbContext context)
//        {
//            _vnPayService = vnPayService;
//			_context = context;
//        }
       
//        [HttpPost("Check-out")]
//        public IActionResult Checkout([FromBody] PaymentRequest model)
//        {
//            if (model.PaymentMethod == "VnPay")
//            {
//                var vnPayModel = new PaymentRequest
//                {
//                    Id = Guid.NewGuid(),
//                    IdUser = Guid.NewGuid(),
//                    OrderId = new Random().Next(1000, 100000),
//                    Amount = model.Amount,
//                    PaymentMethod = model.PaymentMethod,
//                    CreatedDate = DateTime.Now,
//                };

//                // Tạo URL thanh toán cho VNPay
//                var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);

//                // Trả về URL để Blazor điều hướng
//                return Ok(paymentUrl);
//            }
//            return BadRequest("Phương thức thanh toán không hợp lệ.");

//        }

//        [HttpGet("callback")]
//        public IActionResult PaymentCallBack()
//        {
//            var response = _vnPayService.PaymentExecute(Request.Query);

//            if (response == null || response.VnPayResponseCode != "00")
//            {
//                // Trả kết quả thất bại về cho Blazor
//                return BadRequest(new { message = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode}" });
//            }

//            // Nếu thành công, trả kết quả thành công về cho Blazor
//            return Ok(new { message = "Thanh toán VNPay thành công" });
//        }

//        //      [HttpPost("create")]
//        //      public IActionResult CreatePayment([FromBody] PaymentRequest request)
//        //{
//        //	request.Id = Guid.NewGuid();
//        //	request.TransactionDate = DateTime.Now;
//        //	_context.PaymentRequests.Add(request);
//        //	_context.SaveChanges();

//        //	//tạo url thanh toán
//        //	var returnUrl = "https://localhost:5001/api/payment/vnpay_return";
//        //	var paymentUrl = _vnPayService.CreatePaymentUrl(request, returnUrl);

//        //	return Ok(new { paymentUrl } );
//        //}

//        //[HttpGet("vnpay_return")]
//        //public IActionResult VnPayReturn()
//        //{
//        //	var queryParams = HttpContext.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
//        //	var transaction = _vnPayService.ProcessPaymentResponse(queryParams);

//        //	// Lưu thông tin giao dịch vào database
//        //	_context.PaymentTransactions.Add(transaction);
//        //	_context.SaveChanges();

//        //	return Ok(new { message = "Giao dịch thành công", transaction });
//        //}
//    }
//}
