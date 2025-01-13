namespace api.Models.User
{
    public class UserRolesModel
    {
        public bool IsTeacher { get; set; } = false;
        public bool IsStudent { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
    }
}
