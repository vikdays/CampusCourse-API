using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("CampusGroup")]
    public class CampusGroup
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [Key] public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.NameLengthError)]
        public string Name { get; set; }

        public ICollection<CampusCourse> CampusCourses { get; set; }
    }
}
