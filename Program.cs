using Microsoft.EntityFrameworkCore;
using NovaFit.Data;
using NovaFit.Models;

var builder = WebApplication.CreateBuilder(args);

// Veri tabaný baðlantý dizesini yapýlandýrma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yapýlandýrmasý
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Þifre kurallarý 
    options.Password.RequireDigit = false;           // Sayý zorunlu mu? Hayýr
    options.Password.RequireLowercase = false;       // Küçük harf zorunlu mu? Hayýr
    options.Password.RequireUppercase = false;       // Büyük harf zorunlu mu? Hayýr
    options.Password.RequireNonAlphanumeric = false; // Özel karakter (@,!) zorunlu mu? Hayýr
    options.Password.RequiredLength = 3;             // En az kaç karakter? 3 (Test kolaylýðý için)

    // Kullanýcý ayarlarý
    options.User.RequireUniqueEmail = true;          // Her email sadece bir kere kayýt olabilir
})
.AddEntityFrameworkStores<ApplicationDbContext>(); // Identity için veri tabaný deposu olarak ApplicationDbContext'i kullanmasý saðlanýr



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
