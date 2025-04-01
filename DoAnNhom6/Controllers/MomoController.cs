using DoAnNhom6.Models.Order;
using DoAnNhom6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DoAnNhom6.Services;
using DoAnNhom6.ViewModels;
using DoAnNhom6.Models.Momo;
using DoAnNhom6.Helpers;
using System.Security.Policy;
using DoAnNhom6.Models;

namespace DoAnNhom6.Controllers
{
    public class MomoController : Controller
    {
        WebJobContext db = new WebJobContext();
        private readonly ILogger<MomoController> _logger;
        private IMomoService _momoService;

        public MomoController(IMomoService momoService)
        {
            _momoService = momoService;
        }


        public IActionResult Index(string id)
        {
     
            var Amount = new MomoExecuteResponseModel { Amount = id };


            return View(Amount);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);

            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
            
            var amount = int.Parse(response.Amount);
            var a = db.KhachHangs.FirstOrDefault(k => k.MaKh == customerId);

            switch (amount)
            {
                case 500000:
                    a.GoiDangBai = a.GoiDangBai+ 20;
                    break;
                case 300000:
                    a.GoiDangBai = a.GoiDangBai+ 10;
                    break;
                case 100000:
                    a.GoiDangBai = a.GoiDangBai+ 3;
                    break;
                default:
                    // Xử lý khi amount không khớp với bất kỳ giá trị nào trong switch case
                    break;
            }
            db.KhachHangs.Update(a);
            if (response != null)
            {
                var hoaDonGoi = new HoaDonGoi
                {
                    MaHdg = response.OrderId,
                    ThongTin = response.OrderInfo,
                    ThanhToan = float.Parse(response.Amount)
                };
                db.HoaDonGois.Add(hoaDonGoi);
                db.SaveChanges();
            }
          
            db.SaveChanges();




            //var luu_vao_db = response;
            return View(response);

        }

        public IActionResult PaymentPage()
        {
            return View();
        }
    }
}
