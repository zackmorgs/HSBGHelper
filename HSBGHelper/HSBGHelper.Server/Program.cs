using Microsoft.EntityFrameworkCore;
using HSBGHelper.Client;
using HSBGHelper.Server.Components;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<BuddyService>();

builder.Services.AddDbContext<HSBGDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
