namespace Tienda.Recursos {
    public class ProductResourceIn
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Units { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }

    public class ProductResourceOut
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Units { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}