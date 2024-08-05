namespace WebApplication8.Models
{
    public class OrderRequest
    {
        public Checkout ShippingDetail { get; set; }
        public Order Order { get; set; }
    }

}
