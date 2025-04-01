using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom6.Areas.Admin.Controllers
{
    public class BaoLoiController : Controller
    {
        public IActionResult KhongCoQuyen()
        {
            return View();
        }
    }
}
