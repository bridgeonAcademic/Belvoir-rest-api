using Belvoir.DAL.Models;
using System.Text.Json;

namespace Belvoir.GlobalException
{
    public class Exception : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (System.Exception ex)
            {
                var response = new Response<object>
                {
                    statuscode = 500,
                    message = "An unexpected error occurred.",
                    error = ex.Message,
                    data = null
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var responseJson = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(responseJson);

            }
        }
    }
}
