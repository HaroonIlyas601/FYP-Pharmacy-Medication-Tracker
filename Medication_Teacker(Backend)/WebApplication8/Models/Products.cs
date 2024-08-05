namespace WebApplication8.Models
{
    public class Products
    {

        public int id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Images { get; set; }
        public decimal Price { get; set; }
        public decimal PrevPrice { get; set; }
        public string Stock { get; set; }
        public Rating Rating { get; set; }

    }

    public class Rating
    {

        public decimal Rate { get; set; }

        public int Count { get; set; }
    }
}
