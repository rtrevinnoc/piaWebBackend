using System.ComponentModel.DataAnnotations;

namespace Moinitores.Entidades{
    public class Pedido{
    
        public int id {get; set; }

        public int id_user{get; set; }

        public string estado {get; set;}

        public string direccion_envio {get; set;}

        public string metodo_pago {get; set;}
    }
}