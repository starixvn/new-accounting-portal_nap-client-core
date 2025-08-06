namespace Fanex.NAP.Client
{
  using System;
  using System.Net.Http;
  using Fanex.NAP.Client.Clients;
  using Fanex.NAP.Client.Configuration;
  using Fanex.NAP.Client.Helpers.Web;
  using Fanex.NAP.Client.Services;
  using Fanex.NAP.Client.Session;
  using Microsoft.AspNetCore.Http;
  using Microsoft.Extensions.DependencyInjection;

  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddNapClient(this IServiceCollection services)
    {
      services.AddSingleton<INapSession, NapSession>();
      services.AddSingleton<INapSettings>(new NapSettings());
      services.AddSingleton<IGroupService, GroupService>();
      services.AddSingleton<IUserService, UserService>();
      services.AddSingleton<ISecurityService, SecurityService>();
      services.AddSingleton<ISiteService, SiteService>();

      services.AddSingleton<ICheckinClient, CheckinClient>();
      services.AddSingleton<IGroupClient, GroupClient>();
      services.AddSingleton<ILoginClient, LoginClient>();
      services.AddSingleton<ILogoutClient, LogoutClient>();
      services.AddSingleton<ISiteClient, SiteClient>();
      services.AddSingleton<ISSOClient, SSOClient>();
      services.AddSingleton<IUserClient, UserClient>();
      services.AddSingleton<IUserInfoClient, UserInfoClient>();

      services.AddHttpClient<ICheckinClient, CheckinClient>(ConfigureClient());
      services.AddHttpClient<IGroupClient, GroupClient>(ConfigureClient());
      services.AddHttpClient<ILoginClient, LoginClient>(ConfigureClient());
      services.AddHttpClient<ILogoutClient, LogoutClient>(ConfigureClient());
      services.AddHttpClient<ISiteClient, SiteClient>(ConfigureClient());
      services.AddHttpClient<ISSOClient, SSOClient>(ConfigureClient());
      services.AddHttpClient<IUserClient, UserClient>(ConfigureClient());
      services.AddHttpClient<IUserInfoClient, UserInfoClient>(ConfigureClient());

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      return services;
    }
    private static Action<IServiceProvider, HttpClient> ConfigureClient()
    {
      return (sp, c) =>
      {
        var napSettings = sp.GetRequiredService<INapSettings>();
        c.BaseAddress = new Uri(napSettings.ServiceUrls);
      };
    }
  }
}