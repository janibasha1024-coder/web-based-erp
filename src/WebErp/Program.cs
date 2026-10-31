using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WebErp.Data;

var builder = WebApplication.CreateBuilder(args);

var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    const int attempts = 20;
    for (var attempt = 1; attempt <= attempts; attempt++)
    {
        try
        {
            await db.Database.EnsureCreatedAsync();
            await DbSeeder.SeedAsync(db);
            break;
        }
        catch (Exception ex) when (attempt < attempts)
        {
            app.Logger.LogWarning(ex, "SQL Server is not ready (attempt {Attempt} of {Attempts}). Waiting...", attempt, attempts);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
