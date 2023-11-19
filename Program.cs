using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using asp_net_web_api.API.Respository;
using asp_net_web_api.API.Mappings;
using asp_net_web_api.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options => {
        options.AddConsole();
});

builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IItemRepository, ItemRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddMvc(options => {
   options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddDbContext<AppDbContext>(options =>{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); 
    options.UseSqlite(connectionString);
});

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory Management API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandlingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
