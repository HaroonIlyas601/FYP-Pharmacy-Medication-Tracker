using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace WebApplication8.Models
{
    public class ProductsContext
    {
        private readonly string connectionString = "Data Source=DESKTOP-ICTRE3P;Initial Catalog=MedicationTracker;Encrypt=False;Integrated Security=True";


        private IConfiguration _Configuration;

        public ProductsContext(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }
        public string GetProductsData()
        {


            List<object> products = new List<object>();

            //var connectionString = _Configuration.GetConnectionString("MyDBConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var rating = new
                {

                };
                while (reader.Read())
                {
                    var product = new
                    {
                        id = reader["ProductId"],
                        title = reader["Title"],
                        description = reader["Description"],
                        category = reader["Category"],
                        type = reader["Type"],
                        images = reader["Images"],
                        stock = reader["Stock"],
                        price = reader["Price"],
                        prevprice = reader["PrevPrice"],
                        rating = new
                        {
                            rate = reader["Rate"],
                            count = reader["Count"]
                        }
                    };

                    products.Add(product);
                }
            }

            return JsonSerializer.Serialize(products);
        }
        public string GetProductById(int productId)
        {
            object product = null;



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products WHERE ProductId = @ProductId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    product = new
                    {
                        id = reader["ProductId"],
                        title = reader["Title"],
                        description = reader["Description"],
                        category = reader["Category"],
                        type = reader["Type"],
                        images = reader["Images"],
                        stock = reader["Stock"],
                        price = reader["Price"],
                        prevprice = reader["PrevPrice"],
                        rating = new
                        {
                            rate = reader["Rate"],
                            count = reader["Count"]
                        }
                    };
                }
            }

            return JsonSerializer.Serialize(product);
        }

        public string GetProductsByKeyword(string keyword)
        {
            List<object> products = new List<object>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products WHERE Title LIKE @Keyword";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var product = new
                    {
                        id = reader["ProductId"],
                        title = reader["Title"],
                        description = reader["Description"],
                        category = reader["Category"],
                        type = reader["Type"],
                        images = reader["Images"],
                        stock = reader["Stock"],
                        price = reader["Price"],
                        prevprice = reader["PrevPrice"],
                        rating = new
                        {
                            rate = reader["Rate"],
                            count = reader["Count"]
                        }
                    };

                    products.Add(product);
                }
            }

            return JsonSerializer.Serialize(products);
        }

        // Method to get products and nearby pharmacies
        public string GetNearbyPharmacies(double userLat, double userLon, double radius)
        {
            List<object> productsWithPharmacies = new List<object>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                WITH NearbyPharmacies AS (
                    SELECT p.ProductId, p.Title, p.Description, p.Category, p.Type, p.Images, p.Stock, p.Price, p.PrevPrice,
                           p.Rate, p.Count, ph.Name AS PharmacyName, ph.Address AS PharmacyAddress, ph.Latitude, ph.Longitude,
                           (6371 * acos(cos(radians(@UserLat)) * cos(radians(ph.Latitude)) * cos(radians(ph.Longitude) - radians(@UserLon)) + sin(radians(@UserLat)) * sin(radians(ph.Latitude)))) AS distance
                    FROM Products p
                    JOIN Pharmacies ph ON p.PharmacyId = ph.PharmacyId
                )
                SELECT * FROM NearbyPharmacies
                WHERE distance < @Radius
                ORDER BY distance";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserLat", userLat);
                        command.Parameters.AddWithValue("@UserLon", userLon);
                        command.Parameters.AddWithValue("@Radius", radius);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new
                                {
                                    id = reader["ProductId"],
                                    title = reader["Title"],
                                    description = reader["Description"],
                                    category = reader["Category"],
                                    type = reader["Type"],
                                    images = reader["Images"],
                                    stock = reader["Stock"],
                                    price = reader["Price"],
                                    prevprice = reader["PrevPrice"],
                                    rating = new
                                    {
                                        rate = reader["Rate"],
                                        count = reader["Count"]
                                    },
                                    pharmacy = new
                                    {
                                        name = reader["PharmacyName"],
                                        address = reader["PharmacyAddress"],
                                        latitude = reader["Latitude"],
                                        longitude = reader["Longitude"],
                                        distance = reader["distance"]
                                    }
                                };

                                productsWithPharmacies.Add(product);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log and handle SQL exceptions
                Console.WriteLine($"SQL Error: {ex.Message}");
                // Optionally, rethrow or handle specific cases
                throw;
            }
            catch (Exception ex)
            {
                // Log and handle general exceptions
                Console.WriteLine($"General Error: {ex.Message}");
                // Optionally, rethrow or handle specific cases
                throw;
            }

            return JsonSerializer.Serialize(productsWithPharmacies);
        }

        public IEnumerable<Products> SearchProductsNearby(string keyword, double lat, double lon, double radius)
        {
            var products = new List<Products>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SearchProductsNearby", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Keyword", keyword);
                    cmd.Parameters.AddWithValue("@Latitude", lat);
                    cmd.Parameters.AddWithValue("@Longitude", lon);
                    cmd.Parameters.AddWithValue("@Radius", radius);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Products
                            {
                                id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                Category = reader.GetString(3),
                                Type = reader.GetString(4),
                                Images = reader.GetString(5),
                                Price = reader.GetDecimal(6),
                                PrevPrice = reader.GetDecimal(7),
                                Stock = reader.GetString(8),
                                Rating = new Rating
                                {
                                    Rate = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9), // Handle potential null values
                                    Count = reader.IsDBNull(10) ? 0 : reader.GetInt32(10) // Handle potential null values
                                }
                            });
                        }
                    }
                }
            }

            return products;
        }



    }
}
