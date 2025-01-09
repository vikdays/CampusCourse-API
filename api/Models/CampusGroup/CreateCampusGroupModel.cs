using System.ComponentModel.DataAnnotations;

namespace api.Models.CampusGroup
{
    public class CreateCampusGroupModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.NameLengthError)]
        public string Name { get; set; }
    }
}
