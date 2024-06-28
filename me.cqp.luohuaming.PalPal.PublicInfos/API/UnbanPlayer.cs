using System;
using System.Net.Http;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class UnbanPlayer
{
    private static readonly string Api = "v1/api/unban";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userid">steamid</param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public static bool Unban(string userid)
    {
        try
        {
            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Post(url, new{ userid });
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            return !string.IsNullOrEmpty(ret);
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("UnbanPlayer", e.Message + e.StackTrace);
        }

        return false;
    }
}