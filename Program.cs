using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext")));
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddScoped<DbContext>(provider => provider.GetService<DataContext>());
        builder.Services.AddControllers();
        builder.Services.AddTransient<IFacturasService, FacturasService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<RouteOptions>(option => option.LowercaseUrls =true);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
    
            app.UseSwagger();
            app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}