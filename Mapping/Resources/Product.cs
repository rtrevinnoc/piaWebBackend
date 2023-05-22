namespace Tienda.Recursos {
    public class ProductResourceIn
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Units { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string Category { get; set; }
    }

    public class ProductResourceOut
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Units { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ProductResourceUpdate
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Units { get; set; }
        public string? Description { get; set; }

        public string? Category { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class ProductUpdate
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Units { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public byte[]? Image { get; set; }
    }

    public class SearchResource
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
    }
}