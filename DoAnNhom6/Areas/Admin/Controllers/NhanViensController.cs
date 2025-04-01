using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnNhom6.Models;
using DoAnNhom6.ViewModels;
using DoAnNhom6.Helpers;
using System.Text.Json;
using OfficeOpenXml;

namespace DoAnNhom6.Areas.Admin.Controllers
{
    [Area("admin")]
    //[Route("admin")]
    [Route("admin/nhanvien")]
    public class NhanViensController : Controller
    {
        WebJobContext db = new WebJobContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("danhmunhanvien")]
        public IActionResult DanhMucNhanVien()
        {
            var userJson = HttpContext.Session.GetString("user");
            

            if (userJson == null)
            {
                return RedirectToAction("DangNhap", "homeadmin", "area");
            }

            else
            {
                NhanVien nvsession = JsonSerializer.Deserialize<NhanVien>(userJson);
                var count = db.PhanQs.SingleOrDefault(m => m.IdNhanVien == nvsession.MaNv && m.IdChucNang == 2);
                var lstCongViec = db.NhanViens.ToList();
                if (count != null)
                {
                    return View(lstCongViec);
                }
                else
                {
                    return Redirect("/Admin/BaoLoi/KhongCoQuyen");
                }
            }
        }
        // GET: Admin/NhanViens/Details/5

        [Route("ThemNhanVienMoi")]
        [HttpGet]
        public IActionResult ThemNhanVienMoi()
        {
            
            return View();
        }
        [Route("ThemNhanVienMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemNhanVienMoi(NhanVien nv)
        {
            if (nv != null)
            {
                db.NhanViens.Add(nv);
                db.SaveChanges();
                return RedirectToAction("danhmucnhanvien", "nhanviens", "area");
            }
            return View(nv);
        }

        [Route("ThemQuen")]
        [HttpGet]
        public IActionResult ThemQuen(int id)
        {
            ViewBag.idChucNang = new SelectList(db.ChucNangs.ToList(), "ID", "TenChucNang");
            ViewBag.idNhanVien = id;

            return View();
        }
        [Route("ThemQuen")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemQuen(PhanQ q)
        {
            if (q != null)
            {
                db.PhanQs.Add(q);
                db.SaveChanges();
                return RedirectToAction("danhmucnhanvien", "nhanviens", "area");
            }
            return View(q);
        }

        [Route("SuaNV")]
        [HttpGet]
        public IActionResult SuaNV(string id)
        {
            //ViewBag.MaLoai = new SelectList(db.Loais.ToList(), "MaLoai", "TenLoai");
            //ViewBag.MaNcc = new SelectList(db.NhaCungCaps.ToList(), "MaNcc", "TenCongTy");
            var nhanvien = db.NhanViens.Find(id);
            return View(nhanvien);
        }
        [Route("SuaNV")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNV(NhanVien nv)
        {
            if (nv != null)
            {
                db.NhanViens.Update(nv);
                db.SaveChanges();
                return RedirectToAction("danhmucnhanvien", "nhanviens", "area");
            }
            return View(nv);
        }

        [Route("Detail")]
        public IActionResult Detail(string id)
        {

            var data = db.NhanViens.SingleOrDefault(p => p.MaNv == id);

            if (data == null)
            {
                return View();
            }
            var result = new ChiTietNhanVien
            {
                MaNV = data.MaNv,
                HoTen = data.HoTen,
                Email = data.Email,
                Pass = data.MatKhau,
             
                
            };
            //var Hoadon = await db.HoaDons.ToListAsync();
            return View(result);
        }
        [Route("Delete")]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            try
            {
                db.Remove(db.NhanViens.Find(id));
                db.SaveChanges();
                TempData["Message"] = "Nhân Viên đã được xóa";
                return RedirectToAction("DanhMucNhanVien", "NhanViens");
            }
            catch
            {
                TempData["Message"] = "Không được xóa Nhân Viên này";
                return RedirectToAction("DanhMucNhanVien", "HomeAdmin");
            }
        }
        [Route("DownloadExcelNV")]
        public IActionResult DownloadExcelNV()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var collection = db.NhanViens.ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Employer");
            Sheet.Cells["A1"].Value = "MaNv";
            Sheet.Cells["B1"].Value = "HoTen";
            Sheet.Cells["C1"].Value = "Email";
            Sheet.Cells["D1"].Value = "MatKhau";
            Sheet.Cells["E1"].Value = "PhanQs";

            int row = 2;
            foreach (var item in collection)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.MaNv;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.HoTen;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Email;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.MatKhau;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PhanQs;
    
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
