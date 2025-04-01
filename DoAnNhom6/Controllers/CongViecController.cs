using DoAnNhom6.Helpers;
using DoAnNhom6.Models;
using DoAnNhom6.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DoAnNhom6.Controllers
{
    public class CongViecController : Controller
    {
        private readonly WebJobContext db;

        public CongViecController(WebJobContext context) {
            db = context;
        }
        public IActionResult Index(int? loai)
        {
            var  congViecs = db.CongViecs.AsQueryable();

            if (loai.HasValue)
            {
                congViecs = congViecs.Where(p => p.MaLoai == loai.Value);
            }

            var result = congViecs.Select(p => new CongViecVM
            {
                MaCV = p.MaCv,
                TenCV = p.TenCv,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                //TenLoai = p.MaLoaiNavigation.TenLoai
            }); 
            return View(result);
        }
        public IActionResult Search(string query, int? minPrice, int? maxPrice)
        {
            var congViecs = db.CongViecs.AsQueryable();

            // Lọc theo từ khóa query
            if (!string.IsNullOrEmpty(query))
            {
                congViecs = congViecs.Where(p => p.TenCv.Contains(query));
            }

            // Lọc theo giá trong khoảng từ min đến max
            if (minPrice != null && maxPrice != null)
            {
                congViecs = congViecs.Where(p => p.DonGia >= minPrice && p.DonGia <= maxPrice);
            }
            else if (minPrice != null)
            {
                congViecs = congViecs.Where(p => p.DonGia >= minPrice);
            }
            else if (maxPrice != null)
            {
                congViecs = congViecs.Where(p => p.DonGia <= maxPrice);
            }

            var result = congViecs.Select(p => new CongViecVM
            {
                MaCV = p.MaCv,
                TenCV = p.TenCv,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                //TenLoai = p.MaLoaiNavigation.TenLoai
            });

            return View(result);
        }

        //public IActionResult SearchMoney(double? minPrice, double? maxPrice)
        //{
        //    var hangHoas = db.CongViecs.AsQueryable();

        //    if (minPrice != null)
        //    {
        //        hangHoas = hangHoas.Where(p => p.DonGia >= minPrice);
        //    }

        //    if (maxPrice != null)
        //    {
        //        hangHoas = hangHoas.Where(p => p.DonGia <= maxPrice);
        //    }

        //    var result = hangHoas.Select(p => new CongViecVM
        //    {
        //        MaCV = p.MaCv,
        //        TenCV = p.TenCv,
        //        DonGia = p.DonGia ?? 0,
        //        Hinh = p.Hinh ?? "",
        //        MoTaNgan = p.MoTaDonVi ?? "",
        //        TenLoai = p.MaLoaiNavigation.TenLoai
        //    });
        //    return View(result);
        //}
        public IActionResult Detail(int id)
        {

            var data = db.CongViecs
                //.Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p=>p.MaCv == id);
            if (data==null)
            {
                TempData["Message"] = $"Khong thay san pham co ma {id}";
                return Redirect("/404");
            }
            var result = new ChiTietCongViecVM
            {
                MaCv = data.MaCv,
                TenHH = data.TenCv,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                DiaChi=data.DiaChi ?? string.Empty,
                SoLuongTon = 10,//tinhsau
                DiemDanhGia = 5,//tinhsau
            };
            return View(result);
        }
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
            var laygoi = db.KhachHangs.Where(m => m.MaKh == customerId).Select(m => m.GoiDangBai).FirstOrDefault();
            var couthanghoa = db.CongViecs.Count(m => m.MaKh == customerId && m.NgayDangBai.Date == DateTime.Today);
            if (couthanghoa < laygoi)
            {
                ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");

                ViewBag.MaKh = new SelectList(db.KhachHangs.ToList(), "MaKh", "HoTen");
                return View();
            }
            else
            {
                TempData["Message"] = $"Bạn đã đạt giới hạn đăng bài";
                return Redirect("/404");
            }
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken] //kiểm tra dữ liệu được nhập vào có chính xác với quy định của validate k
        public IActionResult Create(CongViec congviec, IFormFile Hinh) 
        {
            //tôi mất 7 tiếng dể xử lý chỗ ModelState.IsValid gì gì đó má, nó sai ác thật k hiểu 
            if (congviec !=null)
            {
                if (Hinh != null)
                {
                    congviec.Hinh = MyUntil.UploadHinh(Hinh, "CongViec");

                }
                //db.CongViecs.Add(congviec);
                db.Add(congviec);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = $"Không thêm được sản phẩm";
                return Redirect("/404");
            }
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
            var sanpham = db.CongViecs.Find(id);
            return View(sanpham);
        }
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken] //kiểm tra dữ liệu được nhập vào có chính xác với quy định của validate k
        public IActionResult Edit(CongViec congviec)
        {
            //tôi mất 7 tiếng dể xử lý chỗ ModelState.IsValid gì gì đó má, nó sai ác thật k hiểu 
            if (congviec != null)
            {
                db.Update(congviec);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = $"Không sửa được sản phẩm";
                return Redirect("/404");
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await db.CongViecs
                .Include(h => h.MaLoai)
                .FirstOrDefaultAsync(m => m.MaCv == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // POST: CongViecs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangHoa = await db.CongViecs.FindAsync(id);
            if (hangHoa != null)
            {
                try
                {
                    db.CongViecs.Remove(hangHoa);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Message"] = $"Không được xóa sản phẩm";
                    return Redirect("/404");
                }
            }

            
            return RedirectToAction(nameof(Index));
        }
    }
}
    