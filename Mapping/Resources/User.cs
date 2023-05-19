using Monitores.Entidades;

namespace Monitores.Recursos {

    public class UserResource
    {
        public string UserName { get; set; }
    }
    public class UserSign
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? EMail { get; set; }
        public Role? Role { get; set; }
    }
}