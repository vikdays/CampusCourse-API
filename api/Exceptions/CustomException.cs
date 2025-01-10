namespace api.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }

        public CustomException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class BadRequestException : CustomException
    {
        public BadRequestException(string message)
            : base(message, 400) { }
    }

    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException(string message)
            : base(message, 401) { }
    }

    public class ForbiddenException : CustomException
    {
        public ForbiddenException(string message)
            : base(message, 403) { }
    }

    public class NotFoundException : CustomException
    {
        public NotFoundException(string message)
            : base(message, 404) { }
    }

    public class InternalServerErrorException : CustomException
    {
        public InternalServerErrorException(string message)
            : base(message, 500) { }
    }
}
