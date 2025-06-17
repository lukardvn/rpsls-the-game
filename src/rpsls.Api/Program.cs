using rpsls.Api;
using rpsls.Api.Endpoints;
using rpsls.Api.Middlewares;
using rpsls.Application;
using rpsls.Domain;
using rpsls.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterApiServices(builder.Configuration);
builder.Services.RegisterApplicationServices();
builder.Services.RegisterDomainServices();
builder.Services.RegisterInfrastructureServices();

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

app.Run();