using System;
using System.Collections.Generic;

namespace DoAnNhom6.Models;

public partial class ChucNang
{
    public int Id { get; set; }

    public string? TenChucNang { get; set; }

    public string? MaChucNang { get; set; }

    public virtual ICollection<PhanQ> PhanQs { get; set; } = new List<PhanQ>();
}
