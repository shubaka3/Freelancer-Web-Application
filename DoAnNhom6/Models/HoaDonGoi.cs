using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class HoaDonGoi
{
    public string MaHdg { get; set; } = null!;

    public string ThongTin { get; set; } = null!;

    public double? ThanhToan { get; set; }
}
