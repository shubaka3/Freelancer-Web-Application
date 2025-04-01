using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string? MatKhau { get; set; }

    public string HoTen { get; set; } = null!;

    public bool GioiTinh { get; set; }

    public DateTime NgaySinh { get; set; }

    public string? DiaChi { get; set; }

    public string? DienThoai { get; set; }

    public string Email { get; set; } = null!;

    public string? Hinh { get; set; }

    public bool HieuLuc { get; set; }

    public bool VaiTro { get; set; }

    public string? RandomKey { get; set; }

    public int? DanhGia { get; set; }

    public int? GoiDangBai { get; set; }

    public string? TrangThai { get; set; }

    public DateTime NgayTaoAcc { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
