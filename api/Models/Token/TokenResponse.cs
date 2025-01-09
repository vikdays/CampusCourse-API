using System.ComponentModel.DataAnnotations;

public class TokenResponse
{
    [Required(ErrorMessage = ErrorConstants.TokenError)]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.TokenLengthError)]
    public string Token { get; set; }

}