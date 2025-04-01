using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class PhanQ
{
    public string IdNhanVien { get; set; } = null!;

    public int IdChucNang { get; set; }

    public string? GhiChu { get; set; }

    public virtual ChucNang IdChucNangNavigation { get; set; } = null!;

    public virtual NhanVien IdNhanVienNavigation { get; set; } = null!;
}
