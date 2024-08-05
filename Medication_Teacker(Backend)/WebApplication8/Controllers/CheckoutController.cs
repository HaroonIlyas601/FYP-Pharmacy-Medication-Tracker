using Microsoft.AspNetCore.Mvc;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : Controller
    {
        private readonly CheckoutContext _context;

        public CheckoutController(CheckoutContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult<Checkout>> PostCheckout(Checkout checkout)
        {
             _context.AddCheckoutAsync(checkout);
            return CreatedAtAction("GetCheckout", new { id = checkout.Id }, checkout);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Checkout>> GetCheckout(int id)
        {
            var checkout = await _context.GetCheckoutAsync(id);

            if (checkout == null)
            {
                return NotFound();
            }

            return checkout;
        }

    }
}
