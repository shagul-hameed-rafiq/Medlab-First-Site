using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Data.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ PostgreSQL instead of SQLite
builder.Services.AddDbContext<MedlabAinsightDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    options.UseNpgsql(cs, x => x.MigrationsAssembly("MedLabAInsights.Data"));
});

var app = builder.Build();

// Swagger (optional but fine)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Always run migrations (important for Render)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MedlabAinsightDbContext>();
    await db.Database.MigrateAsync();
    await Seeder.SeedAsync(db);
}

app.UseHttpsRedirection();
app.MapControllers();

// ✅ CRITICAL for Render
app.Urls.Add("http://0.0.0.0:8080");

app.Run();