using System.Text;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ApplicationTest.Data;
using ApplicationTest.Mapping;
using ApplicationTest.Repositories;
using ApplicationTest.Services;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHttpClient("externalHost", (sp, c) =>
{
    var baseUrl = builder.Configuration["ExternalHost:BaseUrl"]
        ?? throw new InvalidOperationException("Missing ExternalHost:BaseUrl");
    c.BaseAddress = new Uri(baseUrl);
});

// ============= REPOSITORIES =============
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// generic repo: IRepository<T> -> EfRepository<T>
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// spesifik user repo
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ============= SERVICES =============
// Auth
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Brand & Category
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IHostProductService, HostProductService>();
builder.Services.AddScoped<IExternalAuthService, ExternalAuthService>();

// Product + decorator cache
builder.Services.AddScoped<ProductService>();   // concrete
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IProductService>(sp =>
{
    var inner = sp.GetRequiredService<ProductService>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    return new ProductServiceCacheDecorator(inner, cache);
});

// ============= MISC =============
builder.Services.AddAutoMapper(typeof(AppProfile));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.MapInboundClaims = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            NameClaimType = JwtRegisteredClaimNames.Sub,
        };
    });

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
