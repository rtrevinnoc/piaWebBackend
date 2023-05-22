using System.ComponentModel.DataAnnotations;
using Tienda.Entidades;

namespace Tienda.Recursos {
    public class OrderResource
    {
        public Guid OrderId { get; set; }
        public Status Status { get; set; }
    }
    public class PaymentResource
    {
        public Guid OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Address { get; set; }
    }
}