using System.ComponentModel.DataAnnotations;
using api.Models.Enums;

public class UserRegisterModel
{

    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.NameLengthError)]
    public string FullName { get; set; }

    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    public GenderEnum Gender { get; set; }

    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.EmailLengthError)]
    [EmailAddress(ErrorMessage = ErrorConstants.EmailNotValid)]
    public string Email { get; set; }

    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    [StringLength(32, MinimumLength = 6, ErrorMessage = ErrorConstants.PasswordLengthError)]
    [RegularExpression(RegexConstants.PasswordRegex, ErrorMessage = ErrorConstants.PasswordNotValid)]
    public string Password { get; set; }

    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    [StringLength(32, MinimumLength = 6, ErrorMessage = ErrorConstants.PasswordLengthError)]
    [Compare("Password", ErrorMessage = "Passwords must be identical.")]
    public string ConfirmPassword { get; set; }

}