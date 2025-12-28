using System.Text.Json.Serialization;
using Hangfire;
using ProductManagement.Extension;
using ProductManagement.Middilewire;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMiddleware()
    .AddSwagger()
    .AddApplicationServices()
    .InitializeDatabase(builder)
    .AddEmailServices(builder)
    .AddAuthenticationServices(builder)
    .AddHangFireServices(builder)
    .AddGlobalExceptionHandlerServices()
    .AddControllerServices();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();

app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
app.UseSwaggerUI();


app.UseRouting();
app.UseMiddleware<TransactionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();
app.MapControllers();

app.Run();