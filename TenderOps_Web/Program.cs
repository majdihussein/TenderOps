using TenderOps_Web;
using TenderOps_Web.Extensions;
using TenderOps_Web.Services;
using TenderOps_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(u => u.Filters.Add(new AuthExceptionRedirection()));// Add services to the container
builder.Services.AddAutoMapper(typeof(MappingConfig)); // majdi add automapper

builder.Services.AddHttpClient<ITenderService, TenderService>(); // majdi add httpclient factory
builder.Services.AddHttpClient<IInvoiceService, InvoiceService>();
builder.Services.AddHttpClient<IPartnerService, PartnerService>();
builder.Services.AddHttpClient<ITenderCategoryService, TenderCategoryService>(); // majdi add httpclient factory

builder.Services.AddHttpClient<IAuthService, AuthService>();

builder.Services.AddScoped<IBaseService, BaseService>(); // majdi add http BaseService

builder.Services.AddScoped<ITenderService, TenderService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<ITenderCategoryService, TenderCategoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ITokenProvider, TokenProvider>(); // majdi add token provider

builder.Services.AddSingleton<IApiMessageRequestBuilder, ApiMessageRequestBuilder>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // majdi add ContextAccessor

builder.Services.AddDistributedMemoryCache(); // majdi add session 



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set cookie expiration time
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.SlidingExpiration = true; // Enable sliding expiration... 
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

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

app.UseAuthentication(); 
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
