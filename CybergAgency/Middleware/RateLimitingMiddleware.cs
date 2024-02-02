namespace CybergAgency.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static Dictionary<string, (int RequestCount, DateTime LastRequest)> _requestCounts = new Dictionary<string, (int, DateTime)>();

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();

            if (!path.EndsWith(".css") && !path.EndsWith(".js") && !path.EndsWith(".jpg") && !path.EndsWith(".png") && !path.EndsWith("favicon.ico"))
            {
                var ipAddress = context.Connection.RemoteIpAddress.ToString();
                var currentDate = DateTime.UtcNow.Date;

                if (!_requestCounts.ContainsKey(ipAddress))
                {
                    _requestCounts[ipAddress] = (1, currentDate);
                }
                else
                {
                    if (_requestCounts[ipAddress].LastRequest != currentDate)
                    {
                        _requestCounts[ipAddress] = (1, currentDate);
                    }
                    else
                    {
                        _requestCounts[ipAddress] = (_requestCounts[ipAddress].RequestCount + 1, currentDate);
                    }
                }

                if (_requestCounts[ipAddress].RequestCount > 50) 
                {
                    context.Response.StatusCode = 429; 
                    await context.Response.WriteAsync("Daily rate limit exceeded.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
