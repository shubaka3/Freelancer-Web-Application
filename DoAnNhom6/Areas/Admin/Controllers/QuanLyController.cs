using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DoAnNhom6.Models;
using DoAnNhom6.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Scripting;

namespace DoAnNhom6.Areas.Admin.Controllers
{
    [Area("admin")]
    //[Route("admin")]
    [Route("admin/quanly")]
    public class QuanLyController : Controller
    {

        
        WebJobContext db = new WebJobContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("DanhMucQL")]
        public IActionResult DanhMucQL()
        {

            var countnn = db.KhachHangs.Where(m => m.TrangThai == "Nghỉ Ngơi").Count();
            var countss = db.KhachHangs.Where(m => m.TrangThai == "Sẵn Sàng").Count();
            var countdl = db.KhachHangs.Where(m => m.TrangThai == "Đang Làm").Count();
            var trangthai = new TrangThai
            {
               LamViec= countdl,
               NghiNgoi= countnn,
               TimViec= countss
            };

         

            return View(trangthai);
        }
    }
}
