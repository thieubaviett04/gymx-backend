using GymX.Api.Common.Extensions;
using GymX.Application;
using GymX.Application.Common;
using GymX.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiMiddleware();
app.Run();
