using IS_PartnerPolicy.Repository;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Dodajemo repository kao servis
builder.Services.AddScoped<PartnerRepository>(provider =>
{
    // Uzima ConnectionString iz konfiguracije (appsettings.json)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var logger = provider.GetRequiredService<ILogger<PartnerRepository>>();
    return new PartnerRepository(connectionString, logger);
});

var app = builder.Build();

// Config. HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Partner}/{action=Index}/{id?}");

app.Run();
