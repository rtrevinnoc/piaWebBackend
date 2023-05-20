using System.ComponentModel.DataAnnotations;

namespace Monitores.Entidades
{
    public class Productos
    {
        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
        public float precio { get; set; }
        public string descripcion { get; set; }
        public string foto { get; set; }

        public List<Carrito> carrito {get; set;}
    }
}