using api.Exceptions;

namespace api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
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
            catch (ConflictException conflictEx)
            {
                await HandleCustomExceptionAsync(context, conflictEx);
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

            var response = new Response(exception.StatusCode.ToString(), exception.Message);

            return context.Response.WriteAsJsonAsync(response);
        }

        private static Task HandleExceptionsAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new Response("500", exception?.Message ?? "An unexpected error occurred");

            return context.Response.WriteAsJsonAsync(response);
        }
       
    }
}
