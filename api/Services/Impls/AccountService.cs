using api.Entities;
using api.Exceptions;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System.Numerics;

public class AccountService : IAccountService
{
    private readonly DataContext _db;
    private readonly ITokenService _tokenService;
    public AccountService(DataContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }
    public async Task<TokenResponse> Register(UserRegisterModel userRegisterModel)
    {
        if (await _db.Users.FirstOrDefaultAsync(user => user.Email == userRegisterModel.Email) is not null)
        {
            throw new BadRequestException(ErrorConstants.ProfileAlreadyExistsError);
        }
        if (userRegisterModel.Password != userRegisterModel.ConfirmPassword)
        {
            throw new BadRequestException(ErrorConstants.ComparePasswordError);
        }
        if (userRegisterModel.BirthDate > DateTime.UtcNow)
        {
            throw new BadRequestException("Date of birth cannot be in the future.");
        }
        //var hashedPassword = passwordHasher.Generate(userModel.Password);
        var user = UserMapper.MapFromRegisterModelToEntity(userRegisterModel);
        user.Password = BCrypt.Net.BCrypt.HashPassword(userRegisterModel.Password);
        Console.WriteLine(user);

        await _db.Users.AddAsync(user);
        var role = new Role
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            IsAdmin = false
        };
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();

        var token = new TokenResponse { Token = _tokenService.GenerateToken(user) };

        return token;

    }

    public async Task<TokenResponse> Login(UserLoginModel userLoginModel)
    {
        if (await _db.Users.FirstOrDefaultAsync(user => user.Email == userLoginModel.Email) is null)
        {
            throw new BadRequestException(ErrorConstants.ProfileNotExistsError);
        }
        var user = await _db.Users.FirstOrDefaultAsync(user => user.Email == userLoginModel.Email);
        if (!BCrypt.Net.BCrypt.Verify(userLoginModel.Password, user.Password)) { 
            throw new BadRequestException(ErrorConstants.PasswordNotExistsError);
        }
        var token = new TokenResponse { Token = _tokenService.GenerateToken(user) };
        return token;
    }

    public async Task<Response> Logout(string token)
    {
        token = _tokenService.ExtractTokenFromHeader(token);
        if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        _db.BannedTokens.Add(new TokenEntity { Token = token });
        await _db.SaveChangesAsync();
        return await Task.FromResult(new Response(null, "Logout successful"));
    }

    public async Task<User> GetUserByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        }

        if (await _tokenService.IsTokenBanned(token))
        {
            throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        }

        var userId = _tokenService.GetIdByToken(token);
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        }

        var user = await _db.Users.FirstOrDefaultAsync(user => user.Id.ToString() == userId);
        if (user == null) throw new BadRequestException(ErrorConstants.ProfileNotExistsError);
        return user;
    }

    public Task<User?> GetUserById(string id)
    {
        return _db.Users.FirstOrDefaultAsync(user => user.Id.ToString() == id);
    }

    public async Task<UserProfileModel> GetProfile(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        }

        if (await _tokenService.IsTokenBanned(token))
        {
            throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        }
        var user = await GetUserByToken(token);
        return UserMapper.MapFromEntityToUserProfileModel(user);
    }

    public async Task<UserProfileModel> EditProfile(UserProfileModel userProfileModel, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        }

        if (await _tokenService.IsTokenBanned(token))
        {
            throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
        }
        var user = await GetUserByToken(token);
        user = UserMapper.MapFromUserProfileModelToEntity(userProfileModel, user);
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return UserMapper.MapFromEntityToUserProfileModel(user);
    }

}