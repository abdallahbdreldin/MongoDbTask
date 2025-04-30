using AspNetCore.Identity.Mongo;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;
using TodayWebApi.BLL.Managers;
using TodayWebAPi.DAL.Data.Context;
using TodayWebAPi.DAL.Data.Identity;
using TodayWebAPi.DAL.Repos.Basket;
using TodayWebAPi.DAL.Repos.Products;
using TodayWebAPi.DAL.Repos.UnitOfWork;
using Role = TodayWebAPi.DAL.Data.Identity.Role;

var builder = WebApplication.CreateBuilder(args);


var mongoConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseName = builder.Configuration["DatabaseSettings:DatabaseName"];


var settings = MongoClientSettings.FromConnectionString(mongoConnectionString);
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(settings));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName);
});

builder.Services.AddSingleton<StoreContext>();

builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddIdentityMongoDbProvider<User, Role, string>(
    identityOptions =>
    {
        identityOptions.Password.RequireDigit = true;
        identityOptions.Password.RequiredLength = 8;
        identityOptions.Password.RequireLowercase = true;
        identityOptions.Password.RequireUppercase = true;
        identityOptions.Password.RequireNonAlphanumeric = false;
    },
    mongoIdentityOptions =>
    {
        mongoIdentityOptions.ConnectionString = $"{mongoConnectionString}/{databaseName}";
    });

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Email,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomPolicy", policy => policy.RequireRole("customer"));
});


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBasketRepo, BasketRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<ITokenManager, TokenManager>();
builder.Services.AddScoped<IOrderManager, OrderManager>();
builder.Services.AddScoped<IPaymentManager, PaymentManager>();
builder.Services.AddScoped<IEmailManager, EmailManager>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()  
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("Authorization");
        });
});

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

//builder.Services.AddHangfire(config =>
//{
//    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
//          .UseSimpleAssemblyNameTypeSerializer()
//          .UseRecommendedSerializerSettings()
//          .UseMongoStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new MongoStorageOptions
//          {
//              MigrationOptions = new MongoMigrationOptions()
//              {
//                  Strategy = MongoMigrationStrategy.Migration,
//                  BackupStrategy = MongoBackupStrategy.None 
//              }
//          });

//builder.Services.AddHangfireServer();


var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseRouting();
app.UseCors("AllowAll");

app.UseIpRateLimiting();
app.UseAuthentication();   
app.UseAuthorization();

//app.UseHangfireDashboard();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<StoreContext>();
        dbContext.InitializeAsync().Wait();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
    }
}

//RecurringJob.AddOrUpdate("test-job", () => Console.WriteLine("Recurring Job Test!"), Cron.Minutely);
//var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Fire and forget"));
//BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("continuation job"));



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    await IdentityDbContextSeed.SeedUserAsync(userManager, roleManager);

    var storeContext = services.GetRequiredService<StoreContext>();
    await storeContext.InitializeAsync();
}

app.Run();
