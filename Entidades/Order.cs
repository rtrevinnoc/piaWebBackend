using System.ComponentModel.DataAnnotations;

namespace Tienda.Entidades {
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        public string Address { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public ICollection<BoughtProduct> Products { get; set; }
        public double Total { get; set; }
    }

    public enum Status {
        placed,
        paid,
        preparation,
        sent,
        received
    }

    public enum PaymentMethod {
        card,
        PayPal
    }
}