using Data.Repository;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Repository.EntityFramework;
using Take5.Models.Models;
using Take5.Services.Contracts;
using Take5.Services.Implementation;
using Take5.Services.Implementation.Violations;
using WebDriverViolation.Data;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Service.Implementation.Email;
using WebDriverViolation.Service.Models.Email;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Contracts.Email;
using WebDriverViolation.Services.Implementation;
using WebDriverViolation.Services.Implementation.Violations;
using WebDriverViolation.Services.Models.hub;
using WebDriverViolation.Services.Models.Utiltise;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SqlCon") ?? throw new InvalidOperationException("Connection string 'AuthDBContextConnection' not found.");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<APPDBContext>(options =>
options.UseSqlServer(connectionString));


builder.Services.AddDbContext<AuthDBContext>(options =>
            options.UseSqlServer(connectionString));

builder.Services.AddDbContext<AuthDBContext>(options =>
{
    options.UseSqlServer(connectionString,
b => b.MigrationsAssembly(typeof(AuthDBContext).Assembly.FullName));
}, ServiceLifetime.Transient);

builder.Services.AddIdentity<AspNetUser, AspNetRole>()
.AddEntityFrameworkStores<AuthDBContext>()
.AddRoles<AspNetRole>()
.AddDefaultUI().AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(AssembleType));
builder.Services.AddScoped<DbContext, APPDBContext>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped<UserManager<AspNetUser>>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IViolationTypeService, ViolationTypeService>();
builder.Services.AddScoped<ITruckService, TruckService>();
builder.Services.AddScoped<IViolationService, ViolationService>();
builder.Services.AddScoped<ITruckRunningTrackingService, TruckRunningTrackingService>();
builder.Services.AddScoped<IViolationNotificationService, ViolationNotificationService>();
builder.Services.AddScoped<IUserViolationNotificationService, UserViolationNotificationService>();
builder.Services.AddScoped<IUserConnectionManager, UserConnectionManager>();
builder.Services.AddScoped<IConfirmationStatusService, ConfirmationStatusService>();
builder.Services.AddScoped<IEmailSender, MailKitEmailSenderService>();
builder.Services.AddScoped<IOutlookSenderService, OutlookSenderService>();
builder.Services.AddScoped<IUserInputValidationService, UserInputValidationService>();
builder.Services.AddScoped<IObjectMappingService, ObjectMappingService>();
builder.Services.AddScoped<IViolationTypeAccuracyLavelService, ViolationTypeAccuracyLavelService>();
builder.Services.AddScoped<ITruckDetailsService, TruckDetailsService>();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DriverViolation", Version = "v1" });
});

//var hosts = builder.Configuration["AllowedHosts"]?
// .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
//if (hosts?.Length > 0)
//{
//    builder.Services.Configure<HostFilteringOptions>(
//        options => options.AllowedHosts = hosts);
//}
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue;
});

builder.Services.AddMvc().AddMvcOptions(options =>
{
    options.MaxModelValidationErrors = 999999;
});

builder.Services.Configure<MailKitEmailSenderOptions>(options =>
{
    options.Host_Address = builder.Configuration["ExternalProviders:MailKit:SMTP:Address"];
    options.Host_Port = Convert.ToInt32(builder.Configuration["ExternalProviders:MailKit:SMTP:Port"]);
    options.Host_Username = builder.Configuration["ExternalProviders:MailKit:SMTP:Account"];
    options.Host_Password = builder.Configuration["ExternalProviders:MailKit:SMTP:Password"];
    options.Sender_EMail = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
    options.Sender_Name = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderName"];
    options.Username = builder.Configuration["ExternalProviders:MailKit:SMTP:Username"];
    options.Host_Username = builder.Configuration["ExternalProviders:MailKit:SMTP:Host_Username"];
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
//builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});
    var app = builder.Build();
    app.UseHostFiltering();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DriverViolation.API");
        c.EnableFilter();
    });

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();;
    app.UseAuthorization();
    app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "sameorigin");
    context.Response.Headers.Add("Referrer-Policy", "same-origin");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode = block");
    //context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'; connect-src 'self'; img-src 'self'; style-src 'self'; object-src 'self';base-uri 'self';form-action 'self' 'unsafe-inline';");
    context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
    context.Response.Headers[HeaderNames.Expires] = "0";
    context.Response.Headers[HeaderNames.Pragma] = "no-cache";
    context.Request.ContentLength = 300;
    context.Session.Clear();
await next();
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}");
    endpoints.MapRazorPages();
    endpoints.MapHub<NotificationHub>("/NotificationHub");
});
app.Run();
