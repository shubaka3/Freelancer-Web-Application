using AutoMapper;
using DoAnNhom6.Models;
using DoAnNhom6.ViewModels;

namespace DoAnNhom6.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            CreateMap<RegisterVM, KhachHang>()
                //map 1 chiều
                //muốn dùng automapper phải đăng ký
            //lớp đích phải tương ứng v ới lớp đầu tiên
            //Map từ regisster sang khách hàng, cột nào cùng tên thì map qua
            .ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen))
            .ReverseMap();
        }
    }
}
