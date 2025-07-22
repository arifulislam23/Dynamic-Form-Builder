using Dynamic_Form_Builder.Interface;
using Dynamic_Form_Builder.Models;
using Dynamic_Form_Builder.Repositories;
using Microsoft.Data.SqlClient;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);


var dbStatus = new DbStatus();

try
{
    using var conn = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    conn.Open();
    dbStatus.IsDbFailed = false;
}
catch
{
    dbStatus.IsDbFailed = true;
}

builder.Services.AddSingleton(dbStatus);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFormRepository, FormRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var dbStatus = context.RequestServices.GetRequiredService<DbStatus>();

    if (dbStatus.IsDbFailed && !context.Request.Path.StartsWithSegments("/Home/ScriptBackup"))
    {
        context.Response.Redirect("/Home/ScriptBackup");
        return;
    }

    await next();
});




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
