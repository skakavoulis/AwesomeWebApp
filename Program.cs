using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics;

using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = Application.Json;

        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var error = feature?.Error;

        if (error is ApplicationException)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                Error = error.Message
            });
        }
        else
        {
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "Something went wrong. Please try again later."
            });
        }
    });
});
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

