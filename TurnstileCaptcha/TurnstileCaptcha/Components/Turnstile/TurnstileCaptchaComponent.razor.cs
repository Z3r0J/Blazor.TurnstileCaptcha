using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using TurnstileCaptcha.Client.Extensions;
using TurnstileCaptcha.Client.Service;
using TurnstileCaptcha.Client.Service.Interface;

namespace TurnstileCaptcha.Components.Turnstile;

public partial class TurnstileCaptchaComponent
{
    private DotNetObjectReference<TurnstileCaptchaComponent> _turnstileCaptchaComponent;
    protected ElementReference Element;

    [Inject] private IJSRuntime _jsRuntime { get; set; }
    [Inject] private IOptions<TurnstileConfiguration> Configuration { get; set; }

    [Inject] private CacheContainer _cacheContainer { get; set; }


    [Parameter] public TurnstileTheme? Theme { get; set; }

    [Parameter] public TurnstileSize? Size { get; set; }

    [Parameter] public TurnstileAppearance? Appearance { get; set; }

    [Parameter] public TurnstileExecution? Execution { get; set; }

    [Parameter] public TurnstileLanguage? Language { get; set; }


    private string WidgetId { get; set; } = string.Empty;

    private TurnstileConfigurationDto TurnstileConfiguration { get; set; }

    private ITurnstileCaptchaService _turnstileCaptchaService;

    protected override async Task OnParametersSetAsync()
    {
        await SetUserParameters();

        await base.OnParametersSetAsync();
    }


    protected override Task OnInitializedAsync()
    {
        _turnstileCaptchaComponent = DotNetObjectReference.Create(this);
        _turnstileCaptchaService = new TurnstileCaptchaService(_jsRuntime);

        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadScript("https://challenges.cloudflare.com/turnstile/v0/api.js?render=explicit");
            await RenderCaptchaAsync();
        }
    }


    private Task SetUserParameters()
    {
        Theme ??= Configuration.Value.Theme;
        Size ??= Configuration.Value.Size;
        Appearance ??= Configuration.Value.Appearance;
        Execution ??= Configuration.Value.Execution;
        Language ??= Configuration.Value.Language;


        return Task.CompletedTask;
    }

    private async Task LoadScript(string path)
    {
        if (_cacheContainer.LoadedScripts.Contains(path)) return;

        await _jsRuntime.InvokeVoidAsync("loadTurnstileScript", path);

        _cacheContainer.LoadedScripts.Add(path);
    }

    private async Task RenderCaptchaAsync()
    {
        TurnstileConfiguration = new TurnstileConfigurationDto(
            Configuration.Value.SiteKey,
            Theme.ToString()?.ToLower(),
            Appearance.ToString()?.ToLower(),
            Execution.ToString()?.ToLower(),
            Size.ToString()?.ToLower(),
            Language?.ToString()
        );

        WidgetId = await _turnstileCaptchaService.RenderAsync(
            _turnstileCaptchaComponent,
            Element,
            TurnstileConfiguration
        );
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async ValueTask OnCaptchaResolved(string? response)
    {
        try
        {
            Console.WriteLine(response);

            await _turnstileCaptchaService.ResetAsync(WidgetId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}