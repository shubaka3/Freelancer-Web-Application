using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class Loai
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? Hinh { get; set; }

    public int? DanhGia { get; set; }

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();
}
