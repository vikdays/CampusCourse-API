using api.Models.User;

public abstract class UserMapper
{
    public static User MapFromRegisterModelToEntity(UserRegisterModel userRegisterModel)
    {
        return new User
        {
            Name = userRegisterModel.FullName,
            BirthDate = userRegisterModel.BirthDate.ToUniversalTime(),
            Email = userRegisterModel.Email,
            Gender = userRegisterModel.Gender,
            Id = Guid.NewGuid(),
            Password = userRegisterModel.Password
        };
    }

    public static UserProfileModel MapFromEntityToUserProfileModel(User user)
    {
        return new UserProfileModel()
        {
            FullName = user.Name,
            BirthDate = user.BirthDate,
            Email = user.Email,
        };
    }

    public static UserShortModel MapFromEntityToUserShortModel(User user)
    {
        return new UserShortModel()
        {
            Id = user.Id,
            FullName = user.Name
        };
    }
    public static User MapFromUserProfileModelToEntity(UserProfileModel userProfileModel, User user)
    {
        {
            user.Name = userProfileModel.FullName;
            user.BirthDate = userProfileModel.BirthDate;
            user.Email = userProfileModel.Email;
           
        };
        return user;
    }
}
