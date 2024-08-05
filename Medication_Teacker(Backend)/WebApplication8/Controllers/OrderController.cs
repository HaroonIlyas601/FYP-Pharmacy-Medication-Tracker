using Microsoft.AspNetCore.Mvc;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly CheckoutContext _shippingDetailRepository;
        private readonly OrderContext _context;

        public OrderController(CheckoutContext shippingDetailRepository,OrderContext context)
        {
            _shippingDetailRepository = shippingDetailRepository;
            _context = context;
        }

        [HttpPost]
        [Route("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] OrderRequest orderRequest)
        {
            try
            {
                int shippingDetailId = _shippingDetailRepository.AddCheckoutAsync(orderRequest.ShippingDetail);
                orderRequest.Order.ShippingDetailId = shippingDetailId;
                _context.SaveOrder(orderRequest.Order);

                return Ok(new { message = "Order placed successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error placing order", error = ex.Message });
            }
        }
    }
}
