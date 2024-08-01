using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;
using Newtonsoft.Json;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class AnnounceMessage
{
    private static readonly string Api = "v1/api/announce";
    private static object _lock = new object();
    
    public static bool Announce(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return false;
        }
        try
        {
            lock (_lock)
            {
                Process p = CommonHelper.GetOrFindProcess();
                if (p == null || p.HasExited)
                {
                    return false;
                }
                string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
                string ret = CommonHelper.Post(url, new { message });
                if (string.IsNullOrEmpty(ret))
                {
                    throw new HttpRequestException("Get Result is invalid");
                }
                Thread.Sleep(1000);

                return !string.IsNullOrEmpty(ret);
            }
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("AnnounceMessage", e.Message + e.StackTrace);
        }

        return false;
    }
}