using System.ComponentModel.DataAnnotations;

namespace Tienda.Entidades {
    public class BoughtProduct
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? OrderId { get; set; }
        public string ProductName { get; set; }
        public int Units { get; set; }
        public User User { get; set; }
    }
}