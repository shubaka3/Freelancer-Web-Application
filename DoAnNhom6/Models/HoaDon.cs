using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public string MaKh { get; set; } = null!;

    public DateTime Ngay { get; set; }

    public string? HoTen { get; set; }

    public string DiaChi { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string CachThanhToan { get; set; } = null!;

    public int MaTrangThai { get; set; }

    public string? MaNv { get; set; }

    public string? GhiChu { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual NhanVien? MaNvNavigation { get; set; }
}
