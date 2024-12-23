using FoodStore.Models;
using FoodStore.Repositories;
using FoodStore.Services;
using FoodStore.Services.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddDefaultTokenProviders()
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddCors(options => options.AddPolicy(name: "Config",
    policy =>
    {
        policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }));

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<FoodStore.Services.Admin.IFoodService, FoodStore.Services.Admin.FoodService>();
builder.Services.AddScoped<IFoodCategoryRepository, EFFoodCategoryRepository>();
builder.Services.AddScoped<IFoodRepository, EFFoodRepository>();
builder.Services.AddScoped<IInvoiceRepository, EFInvoiceRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, EFOrderDetailRepository>();
builder.Services.AddScoped<IPaymentRepository, EFPaymentRepository>();
builder.Services.AddScoped<ITableRepository, EFTableRepository>();
builder.Services.AddScoped<IIngredientRepository, EFIngredientRepository>();
builder.Services.AddScoped<IFoodIngredientRepository, EFFoodIngredientRepository>();
builder.Services.AddScoped<IQRCodeRepository, EFQRCodeRepository>();
builder.Services.AddScoped<IBuffetRepository, EFBuffetRepository>();
builder.Services.AddScoped<IBuffetDetailRepository, EFBuffetDetailRepository>();
builder.Services.AddTransient<ISupabaseService, SupabaseService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("Config");

app.MapRazorPages();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{

    /* endpoints.MapControllerRoute(name: "Employee", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
     endpoints.MapControllerRoute(
       name: "default",
       pattern: "{controller=Home}/{action=Index}/{id?}");
     endpoints.MapControllerRoute(
         name: "Customer",
         pattern: "{area=Customer}/{controller=Order}/{id?}");
     endpoints.MapControllerRoute(name: "Admin", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");*/
    endpoints.MapControllerRoute(name: "Employee", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "Admin",
        pattern: "{area=Admin}/{controller}/{action}/{id?}");
    endpoints.MapControllerRoute(
        name: "Customer",
        pattern: "{area=Customer}/{controller=Order}/{id?}");
    endpoints.MapControllerRoute(
        name: "Cashier",
        pattern: "{area=Cashier}/{controller=Home}/{action=Index}/{id?}");
});

app.Run();
