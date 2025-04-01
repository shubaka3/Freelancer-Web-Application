using Microsoft.AspNetCore.Mvc;
using DoAnNhom6.Models;

namespace DoAnNhom6.Areas.Admin.Controllers
{
    public class AccessController : Controller
    {
        WebJobContext db = new WebJobContext();

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }
        [HttpPost]
        public IActionResult Login(NhanVien user) {
            if(HttpContext.Session.GetString("UserName")==null)
            {
                var u=db.NhanViens.Where(x=>x.Email.Equals(user.Email) && x.MatKhau.Equals(user.MatKhau)).FirstOrDefault();
                //nếu nó có trong csdl mới thiết lập session
                if (u != null)
                {
                    HttpContext.Session.SetString("UserName", u.Email.ToString());
                    //thiết lập session tên username với giá trị là email, muốn thiết lập nó phải đảm bảo nó chưa tồn tại == null đầu dòng
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        
        }
    }
}
