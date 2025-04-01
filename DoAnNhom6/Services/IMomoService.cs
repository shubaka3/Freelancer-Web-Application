using DoAnNhom6.Models.Momo;
using DoAnNhom6.Models.Order;

namespace DoAnNhom6.Services;

public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
    MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
}