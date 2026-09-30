using Microsoft.EntityFrameworkCore;
using Flower_Cafe.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var connectionString = builder.Configuration.GetConnectionString("FlowerContext")
    ?? throw new InvalidOperationException("Connection string 'FlowerContext' not found.");

builder.Services.AddDbContext<FlowerContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";

    bool isCreateEditDelete =
        path.Contains("/create") ||
        path.Contains("/edit") ||
        path.Contains("/delete");

    // Public pages and static files
    if (path.StartsWith("/css") ||
        path.StartsWith("/js") ||
        path.StartsWith("/lib") ||
        path.StartsWith("/favicon") ||
        path.StartsWith("/accessdenied") ||
        path.StartsWith("/privacy") ||
        path.StartsWith("/logout") ||
        path == "/" ||
        path == "/index")
    {
        await next();
        return;
    }

    // Public menu access:
    // Everyone can view the menu index/details, even when logged out.
    // But create/edit/delete still require permission below.
    if (path.StartsWith("/menuitempages") && !isCreateEditDelete)
    {
        await next();
        return;
    }

    var isLoggedIn = AuthHelper.IsLoggedIn(context);
    var role = AuthHelper.GetUserType(context);

    if (!isLoggedIn)
    {
        context.Response.Redirect("/");
        return;
    }

    if (role == "admin")
    {
        await next();
        return;
    }

    if (role == "manager")
    {
        if (path.StartsWith("/menuitempages"))
        {
            await next();
            return;
        }

        if (isCreateEditDelete)
        {
            context.Response.Redirect("/AccessDenied");
            return;
        }

        await next();
        return;
    }

    if (role == "employee")
    {
        if (path.StartsWith("/employeespages") || path.StartsWith("/customerpages"))
        {
            context.Response.Redirect("/AccessDenied");
            return;
        }

        if (path.StartsWith("/menuitempages") || path.StartsWith("/orderpages"))
        {
            if (isCreateEditDelete)
            {
                context.Response.Redirect("/AccessDenied");
                return;
            }

            await next();
            return;
        }

        context.Response.Redirect("/AccessDenied");
        return;
    }

    if (role == "customer")
    {
        if (path.StartsWith("/employeespages") || path.StartsWith("/customerpages"))
        {
            context.Response.Redirect("/AccessDenied");
            return;
        }

        if (path.StartsWith("/menuitempages"))
        {
            if (isCreateEditDelete)
            {
                context.Response.Redirect("/AccessDenied");
                return;
            }

            await next();
            return;
        }

        if (path.StartsWith("/orderpages"))
        {
            await next();
            return;
        }

        context.Response.Redirect("/AccessDenied");
        return;
    }

    context.Response.Redirect("/AccessDenied");
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();