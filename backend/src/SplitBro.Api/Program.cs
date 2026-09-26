using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SplitBro.Api.Middleware;
using SplitBro.Application;
using SplitBro.Application.Service;
using SplitBro.Infra.Data;
using SplitBro.Infra.Repo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();  // not mandatory to register - auto
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<SplitAppDbContext>(option =>  // later move to infra as extension method
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHealthChecks().AddDbContextCheck<SplitAppDbContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserManagerService>();

builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<GroupManagerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseExceptionHandler(); // builtin ExceptionHandlerMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();


app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();
