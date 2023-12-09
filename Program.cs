using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using asp_net_web_api.API.Respository;
using asp_net_web_api.API.Mappings;
using asp_net_web_api.API.Middlewares;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using asp_net_web_api.API.Utility;
using System.Reflection;

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
builder.Services.AddTransient<IAccountService, AccountService>();
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = ".NET WEB API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {                                
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. \r\n\r\n Enter the token in the text input below.",
    });
    
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
    var secret = builder.Configuration.GetSection("AppSettings:Key").Value; 
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseCustomExceptionHandlingMiddleware();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment()){
  
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI(c => {
    // c.ConfigObject.AdditionalItems["sendHeaders"] = true;
}
);

// else{
//     //inbuilt middleware
//     app.UseExceptionHandler((options)=>{
//         options.Run(
//             async (context) => {
//                 context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                 var ex = context.Features.Get<IExceptionHandlerFeature>();
//                 context.Response.ContentType = "application/json";
//                 await context.Response.WriteAsync(JsonConvert.SerializeObject(new {error = ex.Error.Message}));
//             }
//         );
//     });
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.MapControllers();

app.Run();
