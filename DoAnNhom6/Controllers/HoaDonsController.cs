using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnNhom6.Models;
using DoAnNhom6.Helpers;
using DoAnNhom6.ViewModels;

namespace DoAnNhom6.Controllers
{
    public class HoaDonsController : Controller
    {
        private readonly WebJobContext db;

        public HoaDonsController(WebJobContext context)
        {
            db = context;
        }

        // GET: HoaDons
        public IActionResult Index()
        {
            var XepHang = db.KhachHangs.Select(nv => new BangXepHangVM
            {
                NhanVien = nv,
                HoTen = nv.HoTen,
                Vaitro = nv.VaiTro,
                LayDiem=nv.DanhGia ?? 0,
                CountHD = db.HoaDons.Count(hd => hd.MaKh == nv.MaKh),
                
            }).OrderByDescending(nv=>nv.LayDiem +nv.CountHD).ToList();
            return View(XepHang);


        }

        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }
    }

        // GET: HoaDons/Create
       
}
