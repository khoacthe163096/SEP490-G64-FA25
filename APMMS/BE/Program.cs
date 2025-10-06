using BLL.vn.fpt.edu.extensions;
using BE.vn.fpt.edu.extensions;
using DAL.vn.fpt.edu.data;
using DAL.vn.fpt.edu.entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "APMMS API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });
    options.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// DbContext + Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Project services
builder.Services.AddAutoMapper(typeof(BLL.vn.fpt.edu.extensions.MappingProfile).Assembly);
builder.Services.AddValidators();
builder.Services.AddBusinessServices();
builder.Services.AddAuthDelegates();

// DAL registrations
builder.Services.AddScoped<DAL.vn.fpt.edu.interfaces.IUserRepository, DAL.vn.fpt.edu.repository.UserRepository>();

// Authentication/Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Middleware
app.UseMiddleware<BE.vn.fpt.edu.middleware.ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("Default");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
