using System.ComponentModel.DataAnnotations;

namespace DoAnNhom6.ViewModels
{
    public class RegisterVM
    {
        [Display(Name ="Tên đăng nhập")]
        // chỉnh tên để hiển thị
        //MaKH,MatKhau,HoTen,GioiTinh,NgaySinh?,DiaChi,DienThoai,Email,Hinh?
        [Required(ErrorMessage ="*")]
        [MaxLength(20,ErrorMessage ="Tối đa 20 ký tự")]
        [DataType(DataType.Password)]
        public string MaKh { get; set; }
        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "*")]
        public string MatKhau { get; set; }

        [Display(Name = "Họ Tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string HoTen { get; set; } 

        public bool GioiTinh { get; set; } = true;

        [Display(Name ="Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Địa Chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 ký tự")]
        public string DiaChi { get; set; }

        [Display(Name = "Điện Thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 ký tự")]
        [RegularExpression(@"0[9875]\d{8}",ErrorMessage ="Chưa đúng định dạng SDT ở VN")]
        //định nghĩa biểu thức python, chỉ cho phép nhận sdt, ban đầu là 0, thiếp theo là 09,08,07 hay 05, và sau đó nữa là 8 số
        public string DienThoai { get; set; }

        [EmailAddress(ErrorMessage ="Chưa đúng định dạng email")]
        public string Email { get; set; } 

        public string? Hinh { get; set; }
        public bool Vaitro { get; set; } = true;
    }
}
