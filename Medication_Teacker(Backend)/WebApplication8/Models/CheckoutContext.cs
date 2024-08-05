using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WebApplication8.Models
{
    public class CheckoutContext
    {
        private readonly string connectionString = "Data Source=DESKTOP-ICTRE3P;Initial Catalog=MedicationTracker;Encrypt=False;Integrated Security=True";

        private IConfiguration _Configuration;
        public CheckoutContext(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public int AddCheckoutAsync(Checkout checkout)

        {

            if (checkout == null)
            {
                throw new ArgumentNullException(nameof(checkout));
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO ShippingDetails (FirstName, LastName, Email, Mobile, Address, City, State, Country, PostalCode, PaymentMethod, VisaCardNumber, JazzCashNumber) 
                               OUTPUT INSERTED.Id
                               VALUES (@FirstName, @LastName, @Email, @Mobile, @Address, @City, @State, @Country, @PostalCode, @PaymentMethod, @VisaCardNumber, @JazzCashNumber)"; ;
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", checkout.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", checkout.LastName);
                    cmd.Parameters.AddWithValue("@Email", checkout.Email);
                    cmd.Parameters.AddWithValue("@Mobile", checkout.Mobile);
                    cmd.Parameters.AddWithValue("@Address", checkout.Address);
                    cmd.Parameters.AddWithValue("@City", checkout.City);
                    cmd.Parameters.AddWithValue("@State", checkout.State);
                    cmd.Parameters.AddWithValue("@Country", checkout.Country);
                    cmd.Parameters.AddWithValue("@PostalCode", checkout.PostalCode);
                    cmd.Parameters.AddWithValue("@PaymentMethod", checkout.PaymentMethod);
                    cmd.Parameters.AddWithValue("@VisaCardNumber", (object)checkout.VisaCardNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@JazzCashNumber", (object)checkout.JazzCashNumber ?? DBNull.Value);


                    
                    return (int)cmd.ExecuteScalar();

                }
            }
        }

        public async Task<Checkout> GetCheckoutAsync(int id)
        {
            Checkout checkout = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT *ShippingDetails FROM  WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            checkout = new Checkout
                            {
                                Id = (int)reader["Id"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                Country = reader["Country"].ToString(),
                                PostalCode = reader["PostalCode"].ToString(),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                VisaCardNumber = reader["VisaCardNumber"].ToString(),
                                JazzCashNumber = reader["JazzCashNumber"].ToString()
                            };
                        }
                    }
                }
            }
            return checkout;
        }
    }
}
