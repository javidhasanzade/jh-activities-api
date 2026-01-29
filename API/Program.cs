using API.Middleware;
using API.SignalR;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure.Media;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddMediatR(opt =>
    {
        opt.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
        opt.AddOpenBehavior(typeof(ValidationBehavior<,>));
    }
);
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(opt =>
{
    opt.ApiToken = builder.Configuration.GetSection("ResendClient")["ApiToken"]!;
});
builder.Services.AddTransient<IResend, ResendClient>();

builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddIdentityApiEndpoints<User>(opt => { opt.User.RequireUniqueEmail = true; })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IsActivityHost", policy => { policy.Requirements.Add(new IsHostRequirement()); });
});
builder.Services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
    .WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>();
app.MapHub<CommentHub>("/comments");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var dbContext = services.GetService<AppDbContext>();
    var userManager = services.GetService<UserManager<User>>();
    await dbContext!.Database.MigrateAsync();
    await DbInitializer.SeedData(dbContext, userManager!);
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
}

app.Run();