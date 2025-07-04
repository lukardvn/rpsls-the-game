using Microsoft.EntityFrameworkCore;
using rpsls.Api;
using rpsls.Api.Endpoints;
using rpsls.Api.Middlewares;
using rpsls.Application;
using rpsls.Domain;
using rpsls.Infrastructure;
using rpsls.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterApiServices(builder.Configuration);
builder.Services.RegisterApplicationServices();
builder.Services.RegisterDomainServices();
builder.Services.RegisterInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterGameEndpoints();

using (var scope = app.Services.CreateScope())
{
    await MigrateDbWithRetry(scope.ServiceProvider);
}

app.Run();
return;

//had to add retry because of the dockerization
//api would spin up and the database wouldn't yet be ready so migrations wouldn't apply which would cause exceptions
async Task MigrateDbWithRetry(IServiceProvider services, int maxRetries = 5, TimeSpan? delay = null)
{
    delay ??= TimeSpan.FromSeconds(5);

    var db = services.GetRequiredService<ApplicationDbContext>();

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            break;
        }
        catch (Exception)
        {
            if (attempt == maxRetries)
            {
                throw;
            }

            await Task.Delay(delay.Value);
        }
    }
}

public abstract partial class Program;