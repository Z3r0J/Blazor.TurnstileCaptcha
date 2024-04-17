using TurnstileCaptcha.Client.Extensions;
using TurnstileCaptcha.Client.Pages;
using TurnstileCaptcha.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddTurnstileConfiguration(configuration =>
{
    configuration.Appearance = TurnstileAppearance.Always;
    configuration.Execution = TurnstileExecution.Render;
    configuration.Language = TurnstileLanguage.English;
    configuration.SiteKey = "0x4AAAAAAAXWK6GP0GgTqQOq";
    configuration.Size = TurnstileSize.Normal;
    configuration.Theme = TurnstileTheme.Dark;
    configuration.SecretKey = "0x4AAAAAAAXWK6LJofrzzx9xvn3QfJENVJs";
});

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
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
