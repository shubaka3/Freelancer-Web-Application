using DoAnNhom6.Helpers;
using DoAnNhom6.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom6.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
           var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            // thường lấy sẽ bị rộng cardkey nên sẽ new mới list cartitem
            // -> hiển thị cart model
            //-> định nghĩa cartpanel trong view compoment
            return View("CartPanel",new CartModel
            {
                Quantity=cart.Sum(p=>p.SoLuong),
                Total =  cart.Sum(p=>p.ThanhTien)
            });
        }
    }
}
