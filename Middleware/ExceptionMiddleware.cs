using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace NewsApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _rateLimit = TimeSpan.FromSeconds(30);
        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment, IMemoryCache memoryCache)
        {
            _next = next;
            _environment = environment;
            _memoryCache = memoryCache;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //Security(context);
                //if (IsAllowed(context) == false)
                //{
                //    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                //    context.Response.ContentType = "application/json";
                //    var respnse = new ApiException((int)HttpStatusCode.TooManyRequests, "Too Many Requests. plz Try Again");
                //    await context.Response.WriteAsJsonAsync(respnse);
                //}

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var response = _environment.IsDevelopment() ?
                    new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace) :
                    new ApiException((int)HttpStatusCode.InternalServerError, ex.Message);

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }

        private bool IsAllowed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress.ToString();
            var cacheKey = $"Rate: {ip}";
            var date = DateTime.Now;

            var (timesTamps, count) = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _rateLimit;
                return (timesTamps: date, count: 0);
            });
            if (date - timesTamps < _rateLimit)
            {
                if (count <= 8)
                {
                    return false;
                }
                _memoryCache.Set(cacheKey, (timesTamps, count), _rateLimit);
            }
            else
            {
                _memoryCache.Set(cacheKey, (timesTamps, count), _rateLimit);
            }
            return true;
        }

        private void Security(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Protection"] = "1;mode=block";
            context.Response.Headers["X-Frame-Options"] = "DENY";


        }
    }
}
