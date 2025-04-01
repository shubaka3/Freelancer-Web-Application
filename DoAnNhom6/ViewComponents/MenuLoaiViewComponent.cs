using DoAnNhom6.Models;
using Microsoft.AspNetCore.Mvc;
using DoAnNhom6.ViewModels;

namespace DoAnNhom6.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly WebJobContext db;

        public MenuLoaiViewComponent(WebJobContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai =lo.MaLoai, 
                TenLoai = lo.TenLoai,   
                DanhGia=lo.DanhGia,
                SoLuong = db.CongViecs.Where(m=>m.MaLoai==lo.MaLoai).Count()
                
            }).OrderBy(p => p.TenLoai);
            return View(data);
        }
    }
}
