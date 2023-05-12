using System.ComponentModel.DataAnnotations;

namespace Monitores.Entidades
{
    public class Branch
    {
        [Key]
        public Guid BranchId { get; set; }

        public Company Company { get; set; }
    }
}
