using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TurnstileCaptcha.Client.Extensions;
using TurnstileCaptcha.Client.Service.Interface;

namespace TurnstileCaptcha.Client.Service;

public class TurnstileCaptchaService : ITurnstileCaptchaService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public TurnstileCaptchaService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _moduleTask = new (() => _jsRuntime.InvokeAsync<IJSObjectReference>(
                       "import", "./TurnstileCaptchaRender.js").AsTask());

    }

    public async ValueTask<string> RenderAsync<TData>(DotNetObjectReference<TData> componentReference, ElementReference widgetElement,
        TurnstileConfigurationDto configuration) where TData : class
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("renderTurnstileCaptcha", componentReference, widgetElement, configuration);
    }

    public ValueTask ExecuteAsync(ElementReference widgetElement) => _jsRuntime.InvokeVoidAsync("turnstile.execute",widgetElement);
    public ValueTask ResetAsync(string widgetId) => _jsRuntime.InvokeVoidAsync("turnstile.reset", widgetId);
    public ValueTask RemoveAsync(string widgetId) => _jsRuntime.InvokeVoidAsync("turnstile.remove", widgetId);


}