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
    [Route("admin/nvkhachhang")]
    public class NVKhachHangController : Controller
    {
        WebJobContext db = new WebJobContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("danhmukhachhang")]
        public IActionResult DanhMucKhachHang()
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
                var lstCongViec = db.KhachHangs.ToList();
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
       
      
        [Route("SuaNV")]
        [HttpGet]
        public IActionResult SuaNV(string id)
        {
            WebJobContext db = new WebJobContext();
            var nhanvien = db.KhachHangs.Find(id);
            
            return View(nhanvien);
        }

        [Route("SuaNV")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNV(KhachHang khach)
        {
            var a = db.KhachHangs.FirstOrDefault(k => k.MaKh == khach.MaKh);
            if (khach != null)
            {
                a.HoTen=khach.HoTen;
               a.DanhGia = khach.DanhGia;
                a.DienThoai = khach.DienThoai;
                a.GoiDangBai = khach.GoiDangBai;
                a.DiaChi = khach.DiaChi;
                a.Email = khach.Email;
                a.HieuLuc=khach.HieuLuc;
                
                db.KhachHangs.Update(a);
                db.SaveChanges();
                return RedirectToAction("danhmuckhachhang");
            }
            return View(khach);
        }

        [Route("DownloadExcelUser")]
        public IActionResult DownloadExcelUser()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var collection = db.KhachHangs.ToList();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("User");
            Sheet.Cells["A1"].Value = "HoTen";
            Sheet.Cells["B1"].Value = "DiaChi";
            Sheet.Cells["C1"].Value = "NgaySinh";
            Sheet.Cells["D1"].Value = "DienThoai";
            Sheet.Cells["E1"].Value = "Email";
            Sheet.Cells["F1"].Value = "GioiTinh";
            int row = 2;
            foreach (var item in collection)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.HoTen;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.DiaChi;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NgaySinh;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.DienThoai;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Email;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.GioiTinh;
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
