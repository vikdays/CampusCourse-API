namespace api.Services.Interfaces;

public interface IAccountService
{
    public Task<TokenResponse> Register(UserRegisterModel userRegisterModel);
    public Task<TokenResponse> Login(UserLoginModel userLoginModel);
    public Task<Response> Logout(string token);
    public Task<UserProfileModel> GetProfile(string? token);
    public Task<UserProfileModel> EditProfile(UserProfileModel userProfileModel, string token);
    public Task<User> GetUserByToken(string token);
    public Task<User?> GetUserById(string id);
}