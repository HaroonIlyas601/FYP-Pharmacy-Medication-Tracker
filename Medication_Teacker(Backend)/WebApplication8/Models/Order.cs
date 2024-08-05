namespace WebApplication8.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ShippingDetailId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
