using TcmbKurDonusturucu.Services;
using TcmbKurDonusturucu.Data;
using TcmbKurDonusturucu.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ITcmbKurServisi, TcmbKurServisi>();
builder.Services.AddHostedService<DovizKuruTamamlamaServisi>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!dbContext.Kullanicilar.Any())
    {
        var geciciSifre = Guid.NewGuid().ToString("N")[..12];
        var hasher = new PasswordHasher<Kullanici>();
        var admin = new Kullanici { KullaniciAdi = "admin" };
        admin.SifreHash = hasher.HashPassword(admin, geciciSifre);

        dbContext.Kullanicilar.Add(admin);
        dbContext.SaveChanges();

        Console.WriteLine("Varsayilan kullanici olusturuldu -> kullanici adi: admin, sifre: " + geciciSifre);
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();