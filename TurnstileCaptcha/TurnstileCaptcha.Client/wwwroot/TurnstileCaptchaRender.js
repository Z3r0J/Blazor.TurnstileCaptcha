
 export function renderTurnstileCaptcha(_dotnetObj, element, options) {

    return turnstile.render(
        '.turnstile-captcha',
        {
            sitekey: options.siteKey,
            theme: options.theme,
            size: options.size,
            language: options.language,
            appearance: options.appearance,
            execution: options.execution,
            callback: function (response) {
                _dotnetObj.invokeMethodAsync('OnCaptchaResolved', response)
            },
        }
    )

}