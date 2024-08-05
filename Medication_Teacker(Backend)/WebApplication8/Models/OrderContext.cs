using Microsoft.Data.SqlClient;

namespace WebApplication8.Models
{
    public class OrderContext
    {
        private readonly string connectionString = "Data Source=DESKTOP-ICTRE3P;Initial Catalog=MedicationTracker;Encrypt=False;Integrated Security=True";

        private IConfiguration _Configuration;

        public OrderContext(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public void SaveOrder(Order order)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand command = new SqlCommand("INSERT INTO Orders (ShippingDetailId, TotalAmount) OUTPUT INSERTED.OrderId VALUES (@ShippingDetailId, @TotalAmount)", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ShippingDetailId", order.ShippingDetailId);
                            command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                            order.OrderId = (int)command.ExecuteScalar();
                        }

                        foreach (var item in order.OrderItems)
                        {
                            using (SqlCommand command = new SqlCommand("INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@OrderId", order.OrderId);
                                command.Parameters.AddWithValue("@ProductId", item.ProductId);
                                command.Parameters.AddWithValue("@Quantity", item.Quantity);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
