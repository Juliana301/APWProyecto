var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // MVC + API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger solo en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Más estándar que MapStaticAssets

app.UseRouting();

app.UseAuthorization();

app.MapControllers(); // API

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();