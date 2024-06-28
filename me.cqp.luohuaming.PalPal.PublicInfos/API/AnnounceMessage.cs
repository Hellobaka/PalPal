using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;
using Newtonsoft.Json;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class AnnounceMessage
{
    private static readonly string Api = "v1/api/announce";

    public static bool Announce(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return false;
        }
        try
        {
            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Post(url, new{ message });
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            return !string.IsNullOrEmpty(ret);
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("AnnounceMessage", e.Message + e.StackTrace);
        }

        return false;
    }
}