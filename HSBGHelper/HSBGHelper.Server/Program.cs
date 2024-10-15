using Microsoft.EntityFrameworkCore;
using HSBGHelper.Server.Components;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Services;
using HSBGHelper.Server.Models;
using Microsoft.AspNetCore.Identity;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;

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

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<HSBGDb>()
    .AddDefaultTokenProviders();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});


builder.Services.AddScoped<AuthProvider>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<RoleManager<IdentityRole<int>>>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddBlazoredSessionStorage();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(HSBGHelper.Client._Imports).Assembly);

app.Run();
