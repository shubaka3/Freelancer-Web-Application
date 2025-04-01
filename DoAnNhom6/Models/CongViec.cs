using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class CongViec
{
    public int MaCv { get; set; }

    public string TenCv { get; set; } = null!;

    public int MaLoai { get; set; }

    public double? DonGia { get; set; }

    public string? Hinh { get; set; }

    public DateTime NgayDangBai { get; set; }

    public string? MoTa { get; set; }

    public string? DiaChi { get; set; }

    public string? MaKh { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual Loai MaLoaiNavigation { get; set; } = null!;
}
