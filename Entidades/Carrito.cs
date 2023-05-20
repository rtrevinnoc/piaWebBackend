using System.ComponentModel.DataAnnotations;
using Moinitores.Entidades;

namespace Monitores.Entidades{
    public class Carrito{
        
        public int id {get; set;}

        public int user_id {get; set;}

        public int producto_id {get; set;}

        public int cantidad {get; set;}

        public float precio_unitario {get; set;}

        public float Total {get; set;}

        public List<Pedido> pedido {get; set;}
    }
}