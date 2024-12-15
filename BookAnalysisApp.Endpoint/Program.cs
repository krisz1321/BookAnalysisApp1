using Microsoft.EntityFrameworkCore;
using BookAnalysisApp.Data;
using BookAnalysisApp.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

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
                 //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Register the DatabaseSeeder service
            builder.Services.AddScoped<DatabaseSeeder>();

            // Register the BookEditor service
            builder.Services.AddScoped<BookEditor>();

            // Configure Identity services
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
                  .AddEntityFrameworkStores<ApplicationDbContext>()
                  .AddDefaultTokenProviders();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<RemoveWordFrequencySchemaFilter>(); // Add the custom filter to remove wordFrequency from Swagger docs
            });

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                seeder.SeedDatabase();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Add the Authentication middleware to the pipeline
            app.UseAuthentication();  // This ensures the authentication middleware is used
            app.UseHttpsRedirection();
            app.UseAuthorization();   // This ensures the authorization middleware is used

            // Map Controllers
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
