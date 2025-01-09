using System.ComponentModel.DataAnnotations;

namespace api.Models.CampusGroup
{
    public class CampusGroupModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public string? Name { get; set; }
    }
}
