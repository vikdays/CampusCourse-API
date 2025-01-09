public interface ITokenService
{
    public string GenerateToken(User user);
    Task<bool> IsTokenBanned(string token);
    string GetIdByToken(string token);
    public string ExtractTokenFromHeader(string authorizationHeader);
    public bool ValidateToken(string? token);
}