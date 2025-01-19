using api.Exceptions;

namespace api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException badRequestEx)
            {
                await HandleCustomExceptionAsync(context, badRequestEx);
            }
            catch (UnauthorizedException unauthorizedEx)
            {
                await HandleCustomExceptionAsync(context, unauthorizedEx);
            }
            catch (ForbiddenException forbiddenEx)
            {
                await HandleCustomExceptionAsync(context, forbiddenEx);
            }
            catch (NotFoundException notFoundEx)
            {
                await HandleCustomExceptionAsync(context, notFoundEx);
            }
            catch (InternalServerErrorException internalServerEx)
            {
                await HandleCustomExceptionAsync(context, internalServerEx);
            }
            catch (Exception ex)
            {
                await HandleExceptionsAsync(context, ex);
            }
        }

        private static Task HandleCustomExceptionAsync(HttpContext context, CustomException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;

            return context.Response.WriteAsync(new
            {
                StatusCode = exception.StatusCode,
                Message = exception.Message
            }.ToString() ?? throw new InvalidOperationException());
        }

        private static Task HandleExceptionsAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return context.Response.WriteAsync(new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = exception?.Message
            }.ToString() ?? throw new InvalidOperationException());
        }
    }
}
