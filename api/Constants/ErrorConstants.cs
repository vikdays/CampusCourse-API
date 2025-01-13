static class ErrorConstants
{
    public const string RequiredFieldError = "Field is required";
    public const string NameLengthError = "Minimum length of name = 1";
    public const string EmailLengthError = "Minimum length of email = 1";
    public const string TokenLengthError = "Minimum length of token = 1";
    public const string TokenError = "Token is required";
    public const string PasswordLengthError = "Minimum length of password = 6 and maximummum length of password = 32";
    public const string ContentLengthError = "Minimum length of content = 1";
    public const string TitleLengthError1 = "Minimum length of title = 1";
    public const string TitleLengthError5 = "Minimum length of title = 5";
    public const string DescriptionLengthError = "Minimum length of description = 1";
    public const string AuthorLengthError = "Minimum length of author = 1";
    public const string TagsLengthError = "Minimum item of tags = 1";
    public const string EmailValidError = "Email is invalid";
    public const string ComparePasswordError = "Passwords must be identical.";
    public const string UnauthorizedError = "Unauthorized";
    public const string NotFoundGroupError = "Group Not Found";
    public const string NotFoundCourseError = "Course Not Found";
    public const string StartYearError = "Start year must be from 2000 to 2029";
    public const string MaximumStudentCount = "Maximum student count must be from 1 to 200";
    public const string ForbiddenError = "User isn't admin";
    public const string ClosedCourse = "Campus course is not open for signing up.";
    public const string SignedUpError = "User is already signed up for this course.";
    public const string EmtyBodyError = "The request body cannot be empty.";
    public const string NotInQueueError = "The student is not in queue. Their status cannot be changed.";
    public const string TeacherSignUpError = "A teacher cannot sign up for their own course.";
    public const string PreviousStatusError = "Course status cannot be changed to a previous one.";
    public const string StudentCannootBeTeacherError = "Course's student cannot be a teacher of this course.";
    public const string ForbiddenTeacherError = "Course's student cannot be a teacher of this course.";
    public const string AlreadyTeacherError = "This user is already teaching at this course.";


    public const string ProfileAlreadyExistsError = "Profile already exist";
    public const string ProfileNotExistsError = "Profile not exist";
    public const string PasswordNotExistsError = "Password not exists";
    public const string EmailNotValid = "Email not valid";
    public const string PasswordNotValid = "Password must include one or more numbers";

}