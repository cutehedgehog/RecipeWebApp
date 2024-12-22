using Microsoft.EntityFrameworkCore;
using RecipeApp.Data;
using RecipeApp.Repositories;
using RecipeApp.Repositories.Interfaces;
using RecipeApp.Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddTransient<IIngredientRepository, IngredientRepository>();

var app = builder.Build();
await DbInitializer.SeedData(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
