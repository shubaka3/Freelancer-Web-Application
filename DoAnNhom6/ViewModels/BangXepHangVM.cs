using DoAnNhom6.Models;

namespace DoAnNhom6.ViewModels
{
    public class BangXepHangVM
    {
        public KhachHang NhanVien {  get; set; }
        public int MaNV { get; set; }
        public string HoTen { get; set; }

        public string DiaChi { get; set; }

       
        public int CountHD { get; set; }
        public int LayDiem { get; set; }
        public bool Vaitro { get; set; }
        public int Diem => CountHD + LayDiem;



    }
}
