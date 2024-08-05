using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;
using WebApplication8.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;
using Newtonsoft.Json;

namespace WebApplication8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        public readonly ProductsContext _productsContext;

        public ProductsController(ProductsContext productsContext)
        {
            _productsContext = productsContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Products>> GetProducts()
        {
            var products = SendRequestToSocketServer("GetProducts");
            var product = _productsContext.GetProductsData();
            return Ok(product);
        }

        [HttpGet("{id}")]
        public ActionResult<Products> GetProductById(int id)
        {
            var product = SendRequestToSocketServer($"GetProductById:{id}");
            var products= _productsContext.GetProductById(id);
            if (string.IsNullOrEmpty(products))
            {
                return NotFound();
            }
            return Ok(products);
        }
        [HttpGet("search")]
        public ActionResult<IEnumerable<Products>> SearchProducts([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest("Query parameter 'q' is required.");
            }

            var products = _productsContext.GetProductsByKeyword(q);

            return Ok(products);
        }
        [HttpGet("nearby")]
        public ActionResult<IEnumerable<Products>> GetNearbyPharmacies(double lat, double lon, double radius)
        {
            var products = _productsContext.GetNearbyPharmacies(lat, lon, radius);
            return Ok(products);
        }
        [HttpGet("search/nearby")]
        public ActionResult<IEnumerable<Products>> SearchProductsNearby([FromQuery] string q, [FromQuery] double lat, [FromQuery] double lon, [FromQuery] double radius)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest("Query parameter 'q' is required.");
            }
            // Parse the response from the socket server
            

            var products = SendRequestToSocketServer($"SearchProductsNearby:{q}:{lat}:{lon}:{radius}");
            var product = _productsContext.SearchProductsNearby(q, lat, lon, radius);
            var productsFromSocketServer = ParseProductsFromResponse(products);
            return Ok(product);
        }

        private string SendRequestToSocketServer(string request)
        {
            using (TcpClient client = new TcpClient("10.8.157.1", 9000)) // Ensure the socket server is running on this IP and port
            using (NetworkStream stream = client.GetStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(request);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }
        private IEnumerable<Products> ParseProductsFromResponse(string response)
        {
            try
            {
                // Try to deserialize the response
                return JsonConvert.DeserializeObject<IEnumerable<Products>>(response);
            }
            catch (JsonReaderException ex)
            {
                // Log or handle the error
                Console.WriteLine("Error deserializing response: " + ex.Message);

                // Return an empty list or handle accordingly
                return new List<Products>();
            }
        }
    }
}
