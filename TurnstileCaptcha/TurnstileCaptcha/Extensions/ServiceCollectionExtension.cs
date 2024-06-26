﻿using System.Text.Json.Serialization;
using TurnstileCaptcha.Client.Service;
using TurnstileCaptcha.Client.Service.Interface;
using TurnstileCaptcha.Service;

namespace TurnstileCaptcha.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTurnstileConfiguration(this IServiceCollection service,
        Action<TurnstileConfiguration> configuration)
    {
        service.Configure(configuration);
        service.AddScoped<CacheContainer>();
        service.AddScoped<ITurnstileCaptchaService, TurnstileCaptchaService>();


        return service;
    }
}

public class CacheContainer
{
    public HashSet<string> LoadedScripts { get; }

    public CacheContainer()
    {
        LoadedScripts = new HashSet<string>();
    }
}

public class TurnstileConfiguration
{
    public string SiteKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;


    public TurnstileTheme Theme { get; set; } = TurnstileTheme.Light;

    public TurnstileAppearance Appearance { get; set; }

    public TurnstileExecution? Execution { get; set; }

    public TurnstileSize Size { get; set; }

    public TurnstileLanguage Language { get; set; }

}

public enum TurnstileSize
{
    Normal,
    Compact
}

public enum TurnstileExecution
{
    Render,
    Execute
}

public enum TurnstileAppearance
{
    Always,
    Execute,
    InteractionOnly
}

public enum TurnstileTheme
{
    Light,
    Dark,
    Auto
}

public record TurnstileLanguage(string Language)
{

    public static TurnstileLanguage Spanish = new("es");
    public static TurnstileLanguage English = new("en");
    public static TurnstileLanguage French = new("fr");
    public static TurnstileLanguage German = new("de");
    public static TurnstileLanguage Italian = new("it");
    public static TurnstileLanguage Japanese = new("ja");
    public static TurnstileLanguage Korean = new("ko");
    public static TurnstileLanguage Portuguese = new("pt");

    public override string ToString() => Language;
}

public record TurnstileConfigurationDto(
    string SiteKey,
    string? Theme,
    string? Appearance,
    string? Execution,
    string? Size,
    string? Language
);


public record TurnstileRequestModel(string Response);

public class TurnstileResponseModel
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public string? ChallengeTs { get; set; }

    [JsonPropertyName("")]

    public string? Hostname { get; set; }

    [JsonPropertyName("error-codes")]

    public List<string?> ErrorCodes { get; set; }



}