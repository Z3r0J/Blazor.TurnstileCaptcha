using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using TurnstileCaptcha.Client.Service.Interface;
using TurnstileCaptcha.Extensions;

namespace TurnstileCaptcha.Service;

public class TurnstileCaptchaService : ITurnstileCaptchaService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    private readonly IOptions<TurnstileConfiguration> _turnstileConfiguration;

    public TurnstileCaptchaService(IJSRuntime jsRuntime, IHttpClientFactory httpClientFactory, IOptions<TurnstileConfiguration> turnstileConfiguration, IHttpContextAccessor contextAccessor)
    {
        _jsRuntime = jsRuntime;
        _httpClientFactory = httpClientFactory;
        _turnstileConfiguration = turnstileConfiguration;
        _contextAccessor = contextAccessor;
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

    public Task<TurnstileResponseModel?> ValidateCaptchaAsync(TurnstileRequestModel requestModel)
    {
        var httpClient = _httpClientFactory.CreateClient("TurnstileClient");

        Dictionary<string,string> formRequest = new Dictionary<string,string>()
        {
            {"secret", _turnstileConfiguration.Value.SecretKey},
            {"response", requestModel.Response},
            {"remoteip", _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? ""}
        };


        var request = new HttpRequestMessage(HttpMethod.Post, "/turnstile/v0/siteverify")
        {
            Content = new FormUrlEncodedContent(formRequest)
        };

        return httpClient.SendAsync(request).ContinueWith(response =>
        {
            response.Result.EnsureSuccessStatusCode();

            Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);

            return JsonSerializer.Deserialize<TurnstileResponseModel>(response.Result.Content.ReadAsStringAsync().Result);
        });
    }


}