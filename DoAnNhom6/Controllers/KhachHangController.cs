using AutoMapper;
using DoAnNhom6.Models;
using DoAnNhom6.Helpers;
using DoAnNhom6.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;
using Microsoft.CodeAnalysis.Scripting;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DoAnNhom6.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly WebJobContext db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        // muốn dùng mapper thì cho nó vô
        public KhachHangController(WebJobContext context, IMapper mapper, IConfiguration configuration, IMemoryCache cache)
        {
            db = context;
            _mapper = mapper;
            _configuration = configuration;
            _cache = cache;
        }

        #region Register
        [HttpGet]

        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {
            //if (ModelState.IsValid)
            if(model!=null)
            {
                try
                {
                    if (db.KhachHangs.Any(kh => kh.MaKh == model.MaKh))
                    {
                        ModelState.AddModelError("MaKH", "Mã khách hàng đã tồn tại.");
                        return View(model);
                    }

                    if (db.KhachHangs.Any(kh => kh.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Email đã tồn tại.");
                        return View(model);
                    }

                    var khachHang = _mapper.Map<KhachHang>(model);
                    //map cái model sang kiểu khách hàng
                    //dùng chức năng tạo ra mật khẩu
                    khachHang.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                    khachHang.HieuLuc = true;//xử lý khi có mail để active
                    khachHang.GoiDangBai = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUntil.UploadHinh(Hinh, "KhachHang");

                    }
                    //hàm liên quan tới mật khẩu, mã hóa
                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký.");
                }
            }

            return View();
        }
        #endregion

        #region ForgotPassword
        public void SendMail(string to, string subject, string content)
        {
            var from = _configuration["SMTPConfig:SenderAddress"];
            var displayname = _configuration["SMTPConfig:SenderDisplayName"];
            var pass = _configuration["SMTPConfig:Password"];


            var fromAddress = new MailAddress(from, displayname);
            var toAddress = new MailAddress(to);
            var smtp = new SmtpClient
            {
                Host = _configuration["SMTPConfig:Host"],
                Port = int.Parse(_configuration["SMTPConfig:Port"]),
                EnableSsl = bool.Parse(_configuration["SMTPConfig:EnableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, pass)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(UserViewModel model)
        {
            var viewModel = new UserViewModel
            {
                Register = model.Register,
            };

            var existingUser = await db.KhachHangs.FirstOrDefaultAsync(u => u.Email == model.Register.Email);
            if (existingUser != null)
            {
                string subject = "Forgot Password?";
                string resetToken = Guid.NewGuid().ToString();
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                _cache.Set(resetToken, existingUser.MaKh, cacheOptions);

                //Tạo đường dẫn reset password với token
                string resetPasswordUrl = Url.Action("ResetPassword", "KhachHang", new { makh = existingUser.MaKh, token = resetToken }, Request.Scheme);

                string body = $@"
                    <html>
                    <body>
                        <h2>Are you forgot password?</h2>
                        <p>Please click the button below to reset password:</p>
                        <a href=""{resetPasswordUrl}""><button type=""submit"">Reset password</button></a>
                    </body>
                    </html>";

                SendMail(existingUser.Email, subject, body); // Gọi phương thức gửi email của khách hàng
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Resetpassword(string makh, string token)
        {
            if (_cache.TryGetValue(token, out string userEmail))
            {
                //Xóa token từ bộ nhớ cache sau khi sử dụng
                _cache.Remove(token);

                // Lấy thông tin tài khoản cần đặt lại mật khẩu
                var reset = await db.KhachHangs.FirstOrDefaultAsync(x => x.MaKh == makh);
                if (reset != null)
                {
                    var viewModel = new UserViewModel
                    {
                        Register = reset
                    };
                    return View(viewModel);
                }
            }
            //var reset = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.Email == email);
            //var viewModel = new UserViewModel
            //{
            //    Register = reset,
            //};
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Resetpassword(UserViewModel model)
        {
            var viewModel = new UserViewModel
            {
                Register = model.Register,
            };

            var existingUser = await db.KhachHangs.FirstOrDefaultAsync(u => u.Email == model.Register.Email);
            if (existingUser != null)
            {
                // Cập nhật mật khẩu mới
                existingUser.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.Register.MatKhau);
                await db.SaveChangesAsync(); // Sử dụng await để đợi lưu thay đổi vào cơ sở dữ liệu hoàn tất
                return RedirectToAction("Index", "Home");
            }
            return View(viewModel);
        }
        #endregion

        #region Login in
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        // chức năng login
        //user gửi yêu cầu vô 1 trang -> yêu cầu đăng nhập
        //    -> kiểm tra đăng nhập chưa, islogin = true/false,
        //    sử dụng dbfirst, không dùng dbendenity sẵn có 
        //    nên tự custom,
        //    nếu đã login rồi -> đã login = true -> chuyển tới trang đó
        //    chưa thì đẩy tới trang kiểm tra -> xác thực authenuser -> thành  công -> trang muốn đăng nhập, k thành công -> đá về login
        //    idenity hay dùng retun url( nếu chạy chính xác thì cho chạy vô trang đó luôn, còn nếu chạy tới trang đăng nhập thì lưu lại đường dẫn)

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)// Thêm Task trước IActionResult vì đã có await bên trong
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid) 
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName); //Kiểm tra mã  
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Không có khách hàng này");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
                    }
                    else
                    {
                       
                        if (!BCrypt.Net.BCrypt.Verify(model.Password, khachHang.MatKhau))
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập.");
                        }
                        else
                        {
                            //list thông tin khách hàng
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),

                                //claim = role động
                                new Claim(ClaimTypes.Role,"Customer")
                            };

                            var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal); // await này tồn tại thì phải thêm Task trước IActionResult
                            if (khachHang.VaiTro == false)
                            {
                                return Redirect("ProfileUser");
                                //if (Url.IsLocalUrl(ReturnUrl))
                                //{

                                //        return Redirect(ReturnUrl);

                                //    //Nếu đăng nhập thành công thì chuyển đến trang yêu cầu trước 
                                //}
                                //else
                                //{
                                //    return Redirect("/"); //Nếu đăng nhập không thành công thì chuyển về trang chủ
                                //}
                            }
                            else
                            {
                                return Redirect("ProfileUser");
                                //if (Url.IsLocalUrl(ReturnUrl))
                                //{

                                //    return Redirect("/404");

                                //    //Nếu đăng nhập thành công thì chuyển đến trang yêu cầu trước 
                                //}
                                //else
                                //{
                                //    return Redirect("/404"); //Nếu đăng nhập không thành công thì chuyển về trang chủ
                                //}
                            }

                        }
                    }
                }
            }
            return View();
        }
        #endregion

        [Route("SuaNV")]
        [HttpGet]
        public IActionResult SuaNV(string id)
        {

            WebJobContext db = new WebJobContext();
            var nhanvien = db.KhachHangs.Find(id);

            return View(nhanvien);
        }

        [Route("SuaNV")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNV(KhachHang khach)
        {
            var a = db.KhachHangs.FirstOrDefault(k => k.MaKh == khach.MaKh);
            if (khach != null)
            {
                a.HoTen = khach.HoTen;
                a.DienThoai = khach.DienThoai;
                a.DiaChi = khach.DiaChi;
                a.NgaySinh= khach.NgaySinh;
                a.Email = khach.Email;
                db.KhachHangs.Update(a);
                db.SaveChanges();
                return RedirectToAction("danhmukhachhang");
            }
            return View(khach);
        }
        [Authorize]
        public IActionResult Delete(int? id)
        {
            WebJobContext db = new WebJobContext();
          
                try
                {
                    db.Remove(db.HoaDons.Find(id));
                    db.SaveChanges();
                    TempData["Message"] = "Job đã được xóa";
                    return RedirectToAction("DanhMucCongViec", "HomeAdmin");
                }
                catch
                {
                    TempData["Message"] = "Không được xóa Job này";
                    return RedirectToAction("DanhMucCongViec", "HomeAdmin");
                }
            
        }

        [Authorize]
        public IActionResult ProfileTD()
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
            var Check = db.KhachHangs.Where(h => h.MaKh == customerId).Select(m => m.VaiTro).FirstOrDefault();
            if (Check != true)
            {
                return RedirectToAction("Profile"); // Chuyển hướng đến action ProfileTD
            }

            var danhSachHangHoa = db.CongViecs.Where(h => h.MaKh == customerId).ToList();

            return View(danhSachHangHoa);


        }
        [Authorize]
        public IActionResult DanhSUT(int? id)
        {

            // Truy vấn danh sách mã khách hàng từ bảng hoadon
            //var customerIds = db.HoaDons.Where(hd => hd.MaHd.Any(cthd => cthd.MaHH == id)).Select(hd => hd.MaKH)Distinct().ToList();
            //var cc = db.ChiTietHds.Where(cthd => cthd.MaCv == id).ToList();
    
            var customerIds = db.HoaDons.Where(hd => db.ChiTietHds.Any(cthd => cthd.MaCv == id && cthd.MaHd == hd.MaHd)).Select(hd => hd.MaHd).Distinct().ToList();

            // Lấy thông tin khách hàng từ bảng khachhang

            var customers = db.HoaDons.Where(kh => customerIds.Contains(kh.MaHd)).ToList();


            return View(customers);

        }

        [Authorize]
        public IActionResult UngTuyen(int? id)
        {

            // Truy vấn danh sách mã khách hàng từ bảng hoadon
            //var customerIds = db.HoaDons.Where(hd => hd.MaHd.Any(cthd => cthd.MaHH == id)).Select(hd => hd.MaKH)Distinct().ToList();
            //var cc = db.ChiTietHds.Where(cthd => cthd.MaCv == id).ToList();
            var customerIds = db.HoaDons.Where(hd => db.ChiTietHds.Any(cthd => cthd.MaCv == id && cthd.MaHd == hd.MaHd)).Select(hd => hd.MaKh).Distinct().ToList();

            // Lấy thông tin khách hàng từ bảng khachhang
            var customers = db.KhachHangs.Where(kh => customerIds.Contains(kh.MaKh)).ToList();

            return View(customers);

        }
        [Authorize]
        public IActionResult ProfileUser()
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;

            var khachhang = db.KhachHangs.FirstOrDefault(m=>m.MaKh==customerId);
           
            if (khachhang != null)
            {
                var profile = new ProfileVM
                {
                    HoTen = khachhang.HoTen,
                    GioiTinh = khachhang.GioiTinh ? "Nam" : "Nữ",
                    NgaySinh = khachhang.NgaySinh,
                    DiaChi = khachhang.DiaChi,
                    DienThoai = khachhang.DienThoai,
                    Email = khachhang.Email,
                    Hinh = khachhang.Hinh,
                    HieuLuc = khachhang.HieuLuc,
                    DanhGia = khachhang.DanhGia,
                    GoiDangBai = khachhang.GoiDangBai,
                    TrangThai = khachhang.TrangThai,


                };
                    return View(profile);
            }

            else
            {
                return View();
            }
       
          


        }


        [Authorize]
        public IActionResult Profile()
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;

            var Check = db.KhachHangs.Where(h => h.MaKh == customerId).Select(m => m.VaiTro).FirstOrDefault();
            if (Check != false)
            {
                return RedirectToAction("ProfileTD"); // Chuyển hướng đến action ProfileTD
            }

            var webBanHangContext = db.HoaDons.Include(h => h.MaKhNavigation).Include(h => h.MaNvNavigation).Where(h => h.MaKh == customerId);
            return View(webBanHangContext);


        }
 
        [Authorize]
        public IActionResult XemDScho()
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;

            var Check = db.KhachHangs.Where(h => h.MaKh == customerId).Select(m => m.VaiTro).FirstOrDefault();
          

            var webBanHangContext = db.HoaDons.Include(h => h.MaKhNavigation).Include(h => h.MaNvNavigation).Where(h => h.MaKh == customerId);
            return View(webBanHangContext);


        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }
        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        [Route("DanhGia")]
        [HttpGet]
        public IActionResult DanhGia(string id)
        {

            WebJobContext db = new WebJobContext();
            var nhanvien = db.KhachHangs.Find(id);

            return View(nhanvien);
        }

        [Route("DanhGia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DanhGia(KhachHang khach)
        {
            var a = db.KhachHangs.FirstOrDefault(k => k.MaKh == khach.MaKh);
            if (khach != null)
            {
                a.DanhGia = a.DanhGia + khach.DanhGia;
                db.KhachHangs.Update(a);
                var b = db.HoaDons.FirstOrDefault(h => h.MaKh == khach.MaKh);
                db.Remove(b);
                db.SaveChanges();
                return RedirectToAction("ProfileTD");
            }
            return View(khach);
        }

    }
}




