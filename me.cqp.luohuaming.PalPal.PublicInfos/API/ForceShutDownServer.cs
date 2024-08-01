using System;
using System.Diagnostics;
using System.Net.Http;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class ForceShutDownServer
{
    private static readonly string Api = "v1/api/shutdown";

    public static bool ShutDown()
    {
        try
        {
            Process p = CommonHelper.GetOrFindProcess();
            if (p == null || p.HasExited)
            {
                return false;
            }

            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Post(url, new{  });
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            return !string.IsNullOrEmpty(ret);
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("ForceShutDownServer", e.Message + e.StackTrace);
        }

        return false;
    }
}