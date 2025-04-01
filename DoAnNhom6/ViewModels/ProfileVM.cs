namespace DoAnNhom6.ViewModels
{
    public class ProfileVM
    {
        public string HoTen { get; set; } = null!;

        public string GioiTinh { get; set; }

        public DateTime NgaySinh { get; set; }

        public string? DiaChi { get; set; }

        public string? DienThoai { get; set; }

        public string Email { get; set; } = null!;

        public string? Hinh { get; set; }

        public bool HieuLuc { get; set; }

        public int? DanhGia { get; set; }

        public int? GoiDangBai { get; set; }

        public string? TrangThai { get; set; }

    }
}
