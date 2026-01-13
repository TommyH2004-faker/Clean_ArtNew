using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Application.Behaviors;
using TodoApp.Application.Repository;
using TodoApp.Application.Service;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repository;
using TodoApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add GlobalFilters and Services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<TodoApp.WebAPI.Filters.GlobalExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoAppDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

// Register Repositories
builder.Services.AddScoped<IBookRepository, BookRepositoryImpl>();
builder.Services.AddScoped<IGenreRepository, GenreRepositoryImpl>();
builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();

// Register Domain Event Dispatcher (Auto-discovery cho Event-Driven Architecture)
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// Register Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<UserService>();
//  JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

//  JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
    };
});

builder.Services.AddAuthorization();
//  MediatR - CQRS Pattern + Domain Events
// ở đây đăng ký các handlers từ assembly Application 
// nó sẽ tự đăng ký tất cả các command, query handlers VÀ event handlers
 builder.Services.AddMediatR(cfg => {
        // Register từ Application layer (Commands, Queries, Event Handlers)
        cfg.RegisterServicesFromAssembly(typeof(TodoApp.Application.Features.BookHandle.Command.CreateBookCommand).Assembly);
        
        //  THÊM ValidationBehavior để tự động validate
        cfg.AddOpenBehavior(typeof(TodoApp.Application.Behaviors.ValidationBehavior<,>));
    });
//  FluentValidation
// Tự động đăng ký tất cả các validator từ assembly Application
builder.Services.AddValidatorsFromAssembly(typeof(TodoApp.Application.Features.BookHandle.Command.CreateBookCommand).Assembly);
// Đăng ký MediatR Pipeline Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
 app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
