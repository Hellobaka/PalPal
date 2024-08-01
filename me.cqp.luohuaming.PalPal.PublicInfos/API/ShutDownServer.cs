using System;
using System.Diagnostics;
using System.Net.Http;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class ShutDownServer
{
    private static readonly string Api = "v1/api/shutdown";

    public static bool ShutDown(int waittime, string message)
    {
        try
        {
            Process p = CommonHelper.GetOrFindProcess();
            if (p == null || p.HasExited)
            {
                return false;
            }

            message = message + $" 将于 {waittime} 秒后关闭服务器";
            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Post(url, new{ waittime, message });
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            return !string.IsNullOrEmpty(ret);
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("ShutDownServer", e.Message + e.StackTrace);
        }

        return false;
    }
}