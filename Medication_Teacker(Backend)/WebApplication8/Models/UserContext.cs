using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Fyp.Models
{
    public class UserContext
    {
        
        private readonly IConfiguration _configuration;
        private readonly string connectionString = "Data Source=DESKTOP-ICTRE3P;Initial Catalog=MedicationTracker;Encrypt=False;Integrated Security=True";

        public UserContext(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        public bool RegisterUser(string firstName, string lastName, string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (FirstName, LastName, Email, Password) VALUES (@FirstName, @LastName, @Email, @Password)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Hash password before storing

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }
        public bool ValidateUser(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Hash password before comparing

                connection.Open();
                int result = (int)command.ExecuteScalar();
                return result > 0;
            }
        }
    }
}
