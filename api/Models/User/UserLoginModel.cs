using System.ComponentModel.DataAnnotations;

public record UserLoginModel
{
    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.EmailLengthError)]
    [EmailAddress(ErrorMessage = ErrorConstants.EmailNotValid)]
    public string Email { get; set; }
    [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
    [StringLength(1000, MinimumLength = 6, ErrorMessage = ErrorConstants.PasswordLengthError)]
    [RegularExpression(RegexConstants.PasswordRegex, ErrorMessage = ErrorConstants.PasswordNotValid)]
    public string Password { get; set; }

}