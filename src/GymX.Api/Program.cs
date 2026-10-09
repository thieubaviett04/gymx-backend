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

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GymX.Infrastructure.Persistence.ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    await GymX.Infrastructure.Persistence.ApplicationDbContextSeed.SeedSampleDataAsync(context, logger);
}

app.Run();
