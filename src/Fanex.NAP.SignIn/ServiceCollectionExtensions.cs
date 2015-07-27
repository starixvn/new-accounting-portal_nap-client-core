namespace Microsoft.Extensions.DependencyInjection;

using System;
using Configuration;
using Microsoft.AspNetCore.Http;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Helpers;
using ONEbook.UM.AspNetCore.Filters;
using ONEbook.UM.Configurations;
using ONEbook.UM.Services;
using Microsoft.Extensions.Caching.Memory;
using ONEbook.UM.Transport;

public static class ServiceCollectionExtensions
{
    [ObsoleteAttribute("This method is obsolete. Call AddOneBookUm instead.", true)]
    public static IServiceCollection AddMaintenance(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOneBookUm(configuration);
    }

    public static IServiceCollection AddOneBookUm(this IServiceCollection services, IConfiguration configuration)
    {
        var umConfig = new UMClientConfiguration();
        services.AddSingleton(umConfig.Initialize(configuration));
        services.AddScoped<CheckUMFilterAttribute>();
        services.AddSingleton<IUMCacheService, MemoryUmCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
        services.AddSingleton<IMaintenanceService, MaintenanceService>();

        return services;
    }
}

internal class HttpContextHelper(IHttpContextAccessor httpContextAccessor) : IHttpContextHelper
{
    public string GetSession(string key)
    {
        var value = httpContextAccessor?.HttpContext?.Session.GetString(key);
        return value ?? string.Empty;
    }

    public void SetSession(string key, string value)
    {
        var session = httpContextAccessor?.HttpContext?.Session;
        session?.SetString(key, value);
    }

    public string GetValueFromQueryString(string queryKey)
    {
        var context = httpContextAccessor.HttpContext;
        var value = context == null ? string.Empty : context.Request.Query[queryKey].ToString();
        return value;
    }
}


public class MemoryUmCacheService(IMemoryCache memoryCache) : IUMCacheService
{
    public T Get<T>(string key) => memoryCache.Get<T>(key);

    public void Set<T>(string key, T value, TimeSpan absoluteExpiration)
        => memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration));

    public void Remove(string key) => memoryCache.Remove(key);
}