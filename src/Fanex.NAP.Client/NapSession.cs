namespace Fanex.NAP.Client;

using System.Collections.Generic;
using Fanex.NAP.Client.Session;
using Microsoft.AspNetCore.Http;
using Models;
public interface INapSession
{
    string UserName { get; set; }

    int UserId { get; set; }

    int Level { get; set; }

    int OnUserId { get; set; }

    IList<MenuItem> MenuItemList { get; set; }

    void ClearAll();
}

public class NapSession(IHttpContextAccessor accessor) : INapSession
{
    public const string InternalUsername = "InternalUsername";
    public const string InternalUserId = "InternalUserId";
    public const string InternalOnUserId = "InternalOnUserId";
    public const string InternalNetSessionId = "InternalNetSessionId";
    public const string InternalLevel = "InternalLevel";
    public const string InternalMenuItemList = "InternalMenuItemList";

    public string UserName
    {
        get => accessor.HttpContext.Session?.GetString(InternalUsername) ?? string.Empty;
        set => accessor.HttpContext.Session.SetString(InternalUsername, value);
    }

    public int UserId
    {
        get => accessor.HttpContext.Session?.GetInt32(InternalUserId) ?? 0;
        set => accessor.HttpContext.Session.SetInt32(InternalUserId, value);
    }

    public int OnUserId
    {
        get => accessor.HttpContext.Session?.GetInt32(InternalOnUserId) ?? 0;
        set => accessor.HttpContext.Session.SetInt32(InternalOnUserId, value);
    }

    public int Level
    {
        get => accessor.HttpContext.Session?.GetInt32(InternalLevel) ?? 0;
        set => accessor.HttpContext.Session.SetInt32(InternalLevel, value);
    }

    public string SessionId
    {
        get => accessor.HttpContext.Session?.GetString(InternalNetSessionId) ?? string.Empty;
        set => accessor.HttpContext.Session.SetString(InternalNetSessionId, value);
    }

    public IList<MenuItem> MenuItemList
    {
        get => accessor.HttpContext.Session.Get<List<MenuItem>>(InternalMenuItemList) ?? [];
        set => accessor.HttpContext.Session.Set(InternalMenuItemList, value);
    }

    public void ClearAll()
    {
        accessor.HttpContext.Session.Clear();
    }
}