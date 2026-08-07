using Microsoft.EntityFrameworkCore;
using TcmbKurDonusturucu.Data;
using TcmbKurDonusturucu.Models;

var builder = WebApplication.CreateBuilder(args);

// Controller ve View servislerini ekle
builder.Services.AddControllersWithViews();

// PostgreSQL (AppDbContext) veritabanı bağlantısını ekle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// HTTP request pipeline yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();