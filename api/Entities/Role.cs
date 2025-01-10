using api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("Role")]
    public class Role { 
        [Key] public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
