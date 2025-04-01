using DoAnNhom6.Models;
using DoAnNhom6.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DoAnNhom6.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DoAnNhom6.Controllers
{
    public class CartController : Controller
    {
        private readonly WebJobContext db;

        public CartController(WebJobContext context) 
        {
            db = context;
        }

        
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>> (MySetting.CART_KEY) ?? new List<CartItem> ();
            
        
        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p=>p.MaCv== id);
            if(item == null)
            {
                var hangHoa = db.CongViecs.SingleOrDefault(p=>p.MaCv== id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem 
                { 
                    MaCv = hangHoa.MaCv,
                    TenHH=hangHoa.TenCv,
                    DonGia=hangHoa.DonGia ?? 0,
                    Hinh=hangHoa.Hinh ?? string.Empty,
                    SoLuong=quantity
                };
                gioHang.Add(item);
                
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id) 
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaCv== id);
            if(item != null) 
            {
                gioHang.Remove(item);   
                HttpContext.Session.Set(MySetting.CART_KEY,gioHang);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
           
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }

            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    SoDienThoai = model.DienThoai ?? khachHang.DienThoai,
                    Ngay = DateTime.Now,
                    CachThanhToan = "COD",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaCv = item.MaCv,
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }

            return View(Cart);
        }

    }
}
