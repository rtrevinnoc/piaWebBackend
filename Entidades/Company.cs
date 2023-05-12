using System.ComponentModel.DataAnnotations;

namespace Monitores.Entidades
{
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; }

        public ICollection<Branch> Branches { get; set; }

        public Company()
        {
            this.Branches = new List<Branch>();
        }
    }
}
