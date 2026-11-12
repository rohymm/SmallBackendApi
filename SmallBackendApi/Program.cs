using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Middleware;
using SmallBackendApi.Services;

namespace SmallBackendApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the DbContext and read the connection string from appsettings.json
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register the service: Scoped = one instance per HTTP request
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            // Controllers + Views (supports both API controllers and MVC controllers)
            builder.Services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Custom middleware: placed first so it measures the whole pipeline
            app.UseMiddleware<RequestTimingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // API controllers use attribute routing
            app.MapControllers();

            // MVC controllers use this default route: /Controller/Action/Id
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=EmployeesMvc}/{action=Index}/{id?}");

            app.Run();
        }
    }
}