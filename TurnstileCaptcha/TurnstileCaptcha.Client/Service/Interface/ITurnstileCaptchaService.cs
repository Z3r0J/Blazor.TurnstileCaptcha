using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TurnstileCaptcha.Client.Extensions;

namespace TurnstileCaptcha.Client.Service.Interface;

public interface ITurnstileCaptchaService
{
    ValueTask<string> RenderAsync<TData>(DotNetObjectReference<TData> componentReference,
        ElementReference widgetElement,
        TurnstileConfigurationDto configuration
        ) where TData : class;

    ValueTask ExecuteAsync(ElementReference widgetElement);

    ValueTask ResetAsync(string widgetId);

    ValueTask RemoveAsync(string widgetId);

}