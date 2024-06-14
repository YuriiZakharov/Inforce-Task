using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<DataContext>(options =>
{    
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");    
    options.UseSqlServer(connectionString);
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");    
    app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();


app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.MapControllers(); 

app.Run();