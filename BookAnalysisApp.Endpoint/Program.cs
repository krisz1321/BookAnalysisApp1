using Microsoft.EntityFrameworkCore;
using BookAnalysisApp.Data;
using BookAnalysisApp.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace BookAnalysisApp.Endpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure Entity Framework Core and the ApplicationDbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("BooksDb") // For testing, we use an in-memory database.
                                                       // For real-world, replace it with a real database connection like:
                                                       // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            );

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<RemoveWordFrequencySchemaFilter>(); // Add the custom filter to remove wordFrequency from Swagger docs
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }

    // Custom Swagger Filter to remove wordFrequency from the generated Swagger docs
    public class RemoveWordFrequencySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(Book))
            {
                // Remove the wordFrequency property from the Swagger docs
                schema.Properties.Remove("wordFrequency");
            }
        }
    }
}
