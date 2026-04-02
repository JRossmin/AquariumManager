using AquariumManager.Application.Services;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using AquariumManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AquariumDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseSqlServer(connectionString);
});

// Repositorios
builder.Services.AddScoped<ISpeciesRepository, SpeciesRepository>();
builder.Services.AddScoped<IInventoryLotRepository, InventoryLotRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();

// Servicios de aplicación
builder.Services.AddScoped<ISpeciesService, SpeciesService>();
builder.Services.AddScoped<IInventoryLotService, InventoryLotService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

// Controllers
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
