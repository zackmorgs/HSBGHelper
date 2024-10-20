using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;

using HSBGHelper.Server.Components;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Services;
using HSBGHelper.Server.Models;
using Microsoft.AspNetCore.Routing;

// creat builder
var builder = WebApplication.CreateBuilder(args);

// configure let's encrypt 
builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.ListenAnyIP(443, (portOptions) =>
    {
        portOptions.UseHttps(h =>
        {
            h.UseLettuceEncrypt(kestrel.ApplicationServices);
        });
    });
    kestrel.ListenAnyIP(80);
});

// Add framework to services
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// add ms sql server
builder.Services.AddDbContext<HSBGDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register custom Services
builder.Services.AddScoped<MinionService>();
builder.Services.AddScoped<HeroService>();
builder.Services.AddScoped<SpellService>();
builder.Services.AddScoped<HeroPowerService>();
builder.Services.AddScoped<LesserTrinketService>();
builder.Services.AddScoped<GreaterTrinketService>();

// add identity core
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<HSBGDb>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();

builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddIdentityCore<User>(options => {
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<HSBGDb>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

builder.Services.AddLettuceEncrypt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(HSBGHelper.Client._Imports).Assembly);

app.MapIdentityApi<IdentityUser>();

app.Run();
