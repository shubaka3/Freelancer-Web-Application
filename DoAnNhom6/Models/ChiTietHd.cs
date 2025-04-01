using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class ChiTietHd
{
    public int MaCt { get; set; }

    public int MaHd { get; set; }

    public int MaCv { get; set; }

    public double DonGia { get; set; }

    public int SoLuong { get; set; }

    public virtual CongViec MaCvNavigation { get; set; } = null!;
}
