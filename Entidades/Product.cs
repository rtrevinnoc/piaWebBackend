using System.ComponentModel.DataAnnotations;

namespace Tienda.Entidades {
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Units { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public string Category { get; set; }
    }
}