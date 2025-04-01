using DoAnNhom6.Models;
using DoAnNhom6.Helpers;
using DoAnNhom6.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using OfficeOpenXml;



namespace DoAnNhom6.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
         WebJobContext db = new WebJobContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                return RedirectToAction("DangNhap");
            }
            else
            {
                return View();
            }
           
        }
       
        


        [Route("DanhMucCongViec")]
        public IActionResult DanhMucCongViec()
        {
            var userJson = HttpContext.Session.GetString("user");
         
            var lstCongViec=db.CongViecs.ToList();
            
            if (userJson == null)
            {
                return RedirectToAction("DangNhap");
            }
        
            else
            {
                return View(lstCongViec);
            }
        }

        [Route("ThemCongViecMoi")]
        [HttpGet]
        public IActionResult ThemCongViecMoi()
        {
            WebJobContext db = new WebJobContext();
            var userJson = HttpContext.Session.GetString("user");
            NhanVien nvsession = JsonSerializer.Deserialize<NhanVien>(userJson);

            var count = db.PhanQs.SingleOrDefault(m => m.IdNhanVien == nvsession.MaNv && m.IdChucNang == 1);
            if (count == null)
            {
                return Redirect("/Admin/BaoLoi/KhongCoQuyen");
            }
            else
            {
            ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
                ViewBag.MaKH = new SelectList(db.KhachHangs.ToList(), "MaKH", "HoTen");
                return View();
            }
           
        }
        [Route("ThemCongViecMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemCongViecMoi(CongViec congviec)
        {
            WebJobContext db = new WebJobContext();
            var userJson = HttpContext.Session.GetString("user");
            NhanVien nvsession = JsonSerializer.Deserialize<NhanVien>(userJson);

            var count = db.PhanQs.SingleOrDefault(m => m.IdNhanVien == nvsession.MaNv && m.IdChucNang == 1);
            if (count == null)
            {
                return Redirect("/Admin/BaoLoi/KhongCoQuyen");
            }
            if (congviec!=null)
            {
                db.CongViecs.Add(congviec);
                    db.SaveChanges();
                    return RedirectToAction("DanhMucCongViec");
            }
            return View(congviec);  
        }

        [Route("SuaCongViec")]
        [HttpGet]
        public IActionResult SuaCongViec(int? id)
        {
            WebJobContext db = new WebJobContext();
            var userJson = HttpContext.Session.GetString("user");
            NhanVien nvsession = JsonSerializer.Deserialize<NhanVien>(userJson);
            var count = db.PhanQs.SingleOrDefault(m => m.IdNhanVien == nvsession.MaNv && m.IdChucNang == 2);
            if (count == null)
            {
                return Redirect("/Admin/BaoLoi/KhongCoQuyen");
            }
            else
            {
                ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
                ViewBag.MaKh = new SelectList(db.KhachHangs.ToList(), "MaKh", "HoTen");
                var sanPham=db.CongViecs.Find(id);
                return View(sanPham);
            }
          
        }
        [Route("SuaCongViec")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaCongViec(CongViec congviec)
        {
            
                if (congviec != null)
                {
                    db.CongViecs.Update(congviec);
                    db.SaveChanges();
                    return RedirectToAction("DanhMucCongViec");
                }
                return View(congviec);
            
            
        }

        [Route("Detail")]
        public IActionResult Detail(int id)
        {
            var data = db.CongViecs.SingleOrDefault(p => p.MaCv == id);
            //var data = db.HangHoas.Include(p => p.MaLoai).SingleOrDefault(p => p.MaCv == id);
            //var tenLoai = db.Loais.Where(l => l.MaLoai == data.MaLoai).Select(l => l.TenLoai).SingleOrDefault();
            if (data == null)
            {
                return View();
            }
            var result = new ChiTietCongViecVM
            {
                MaCv = data.MaCv,
                TenHH = data.TenCv,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                NguoiTuyen = data.MaKh,
                //TenLoai = db.Loais.Where(l => l.MaLoai == data.MaLoai).SingleOrDefault(l => l.TenLoai).SingleOrDefault().ToString(),
                SoLuongTon = 10,//tinhsau
                DiemDanhGia = 5,//tinhsau
            };
            return View(result);
        }
        [Route("Delete")]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            WebJobContext db = new WebJobContext();
            var userJson = HttpContext.Session.GetString("user");
            NhanVien nvsession = JsonSerializer.Deserialize<NhanVien>(userJson);

            var count = db.PhanQs.SingleOrDefault(m => m.IdNhanVien == nvsession.MaNv && m.IdChucNang == 3);
            if (count == null)
            {
                return Redirect("/Admin/BaoLoi/KhongCoQuyen");
            }
            else
            {
                try
                {
                    db.Remove(db.CongViecs.Find(id));
                    db.SaveChanges();
                    TempData["Message"] = "Sản phẩm đã được xóa";
                    return RedirectToAction("DanhMucCongViec", "HomeAdmin");
                }
                catch
                {
                    TempData["Message"] = "Không được xóa sản phẩm này";
                    return RedirectToAction("DanhMucCongViec", "HomeAdmin");
                }
            }
        }

        [Route("DangNhap")]
       
        public IActionResult DangNhap()
        {
                return View();
        }
        [Route("DangNhap")]
        [HttpPost]
        public IActionResult DangNhap(string user, string password)
        {
            //check db
            WebJobContext db = new WebJobContext();
            var nhanvien = db.NhanViens.SingleOrDefault(m => m.Email.ToLower() == user.ToLower() && m.MatKhau == password);
            if (nhanvien!=null)
            {
            
                HttpContext.Session.Set("user", nhanvien);
                return RedirectToAction("DanhMucCongViec", "HomeAdmin");
            }
            else
            {
                TempData["error"] = "Tài khoảng đăng nhập không đúng";
                return View();
            }

        }
        [Route("DangXuat")]
        public IActionResult DangXuat()
        {
            //HttpContext.Session.Remove("user");
            Response.Cookies.Delete("user");

            // Hoặc xóa toàn bộ phiên
            HttpContext.Session.Clear();

            return RedirectToAction("Index","HomeAdmin","Areas");
        }
        [Route("DownloadExcelJOB")]
        public IActionResult DownloadExcelJOB()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var collection = db.CongViecs.ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Jobs");
            Sheet.Cells["A1"].Value = "MaCv";
            Sheet.Cells["B1"].Value = "TenCv";
            Sheet.Cells["C1"].Value = "DiaChi";
            Sheet.Cells["D1"].Value = "MoTa";
            Sheet.Cells["E1"].Value = "MaLoai";
            Sheet.Cells["F1"].Value = "NgayDangBai";
            Sheet.Cells["G1"].Value = "DonGia";
            Sheet.Cells["H1"].Value = "MaKh";

            int row = 2;
            foreach (var item in collection)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.MaCv;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.TenCv;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.DiaChi;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.MoTa;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.MaLoai;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.NgayDangBai;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.DonGia;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.MaKh;


                row++;
            }
            string fileName = "User.xlsx";
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Add("content-disposition", "attachment; filename=" + fileName);
            //Response.AddHeader("content-disposition", "attachment: filename=" + fileName);
            byte[] fileContents = Ep.GetAsByteArray();
            //Response.BinaryWrite(Ep.GetAsByteArray());
            return File(fileContents, Response.ContentType, fileName);
        }
    }
}
