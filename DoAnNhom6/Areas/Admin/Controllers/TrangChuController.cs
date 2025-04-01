using DoAnNhom6.Models;
using DoAnNhom6.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static DoAnNhom6.ViewModels.DanhMucQL;

namespace DoAnNhom6.Areas.Admin.Controllers
{
    [Area("admin")]
    //[Route("admin")]
    [Route("admin/trangchu")]
    public class TrangChuController : Controller
    {

        WebJobContext db = new WebJobContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("DanhMucTrangChu")]
        public IActionResult DanhMucTrangChu()
        {
            var doanhthu = db.HoaDonGois.Sum(m => m.ThanhToan);
            var songdk = db.KhachHangs.Count();
            var countdl = db.KhachHangs.Where(m => m.TrangThai == "Đang Làm").Count();
            //---biểu đồ cột top công việc trong tháng ---
            var topJobsByMonthQuery = db.CongViecs
          .AsEnumerable()
          .GroupBy(job => job.NgayDangBai.Month)
          .Select(group => new
          {
              Thang = group.Key,
              TopCongViecs = group
                  .GroupBy(job => job.TenCv)
                  .Select(jobGroup => new { Loai = jobGroup.Key, SoLanXuatHien = jobGroup.Count() })
                  .OrderByDescending(job => job.SoLanXuatHien)
                  .Take(3)
                  .ToList()
          })
          .OrderBy(result => result.Thang)
          .ToList();

            List<TopJobInfo> topJobsByMonth = topJobsByMonthQuery.Select(monthGroup => new TopJobInfo
            {
                Thang = monthGroup.Thang,
                TopCongViecs = monthGroup.TopCongViecs.Select(jobInfo => new CongViecInfo
                {
                    TenCongViec = jobInfo.Loai,
                    SoLanXuatHien = jobInfo.SoLanXuatHien
                }).ToList()
            }).ToList();
            //--biểu đồ tròn---
            var goi100k = db.HoaDonGois.Where(m => m.ThanhToan == 100000).Count();
            var goi300k = db.HoaDonGois.Where(m => m.ThanhToan == 300000).Count();
            var goi500k = db.HoaDonGois.Where(m => m.ThanhToan == 500000).Count();
            var DanhMucQL = new DanhMucQL
            {
                DoanhThu = doanhthu,
                SoNguoiDangKy = songdk,
                SoNguoiDangLamViec = countdl,
                //---- Biểu đồ cột --
                TopJobsByMonth = topJobsByMonth,
                //---- Biểu đồ tròn---
                Goi100k = goi100k,
                Goi300k = goi300k,
                Goi500k = goi500k,
                //--- Tiến độ công việc --
                CongViecChuaDuyet = 10,
                ReportChuaRep = 20

            };


            return View(DanhMucQL);
        }
    }
}
