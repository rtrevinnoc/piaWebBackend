using System.ComponentModel.DataAnnotations;

namespace Monitores.Entidades {
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EMail { get; set; }
        public Role Role { get; set; }
    }

    public enum Role {
        admin,
        buyer
    }
}