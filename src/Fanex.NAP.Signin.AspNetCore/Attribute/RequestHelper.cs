using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace Fanex.NAP.Signin.AspNetCore.Attribute;

internal static class RequestHelper
{
    public static void CloseConnection(AuthorizationFilterContext context)
    {
        if (IsAjaxRequest(context.HttpContext.Request))
        {
            var response = context.HttpContext.Response;
            const string messageContent = "<center><b>You are not authorized to access the requested resource!</b></center>";
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.WriteAsync(messageContent);
        }
        else
        {
            context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public static bool IsAjaxRequest(HttpRequest request)
        => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
           string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
}