using Microsoft.EntityFrameworkCore;
using HSBGHelper.Server.Components;
using HSBGHelper.Server.Data;
using HSBGHelper.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.ListenAnyIP(443, portOptions =>
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
builder.Services.AddScoped<BuddyService>();

builder.Services.AddDbContext<HSBGDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
