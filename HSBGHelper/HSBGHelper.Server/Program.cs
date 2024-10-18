using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;

using HSBGHelper.Server.Components;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Services;
using HSBGHelper.Server.Models;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

    
// Register Services
builder.Services.AddScoped<MinionService>();
builder.Services.AddScoped<HeroService>();
builder.Services.AddScoped<SpellService>();
builder.Services.AddScoped<HeroPowerService>();
builder.Services.AddScoped<LesserTrinketService>();
builder.Services.AddScoped<GreaterTrinketService>();

builder.Services.AddDbContext<HSBGDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<HSBGDb>()
    .AddDefaultTokenProviders();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddCascadingAuthenticationState();

// Configure Identity cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/admin"; // Default login path
    options.AccessDeniedPath = "/access-denied"; // Access denied path
});

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

app.UseAuthentication(); // Ensure this is before UseAuthorization
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(HSBGHelper.Client._Imports).Assembly);

app.Run();
