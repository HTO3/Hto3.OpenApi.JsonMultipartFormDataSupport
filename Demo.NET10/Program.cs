
using Hto3.OpenApi.JsonMultipartFormDataSupport;

namespace Demo.NET10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();            
            builder.Services.AddOpenApi(options =>
            {
                options.AddOperationTransformer<JsonMultipartOperationTransformer>();
            });

            var app = builder.Build();

            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "API v1");
                options.RoutePrefix = "swagger";
            });

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
