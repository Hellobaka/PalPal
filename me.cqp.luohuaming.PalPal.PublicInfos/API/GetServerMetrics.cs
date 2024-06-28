using System;
using System.Net.Http;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;
using Newtonsoft.Json;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class GetServerMetrics
{
    private static readonly string Api = "v1/api/metrics";

    public static ServerMetrics Get()
    {
        try
        {
            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Get(url);
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            var metrics = JsonConvert.DeserializeObject<ServerMetrics>(ret);
            if (metrics.serverfps < 0)
            {
                throw new HttpRequestException($"Get Result is invalid: {ret}");
            }

            return metrics;
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("GetServerMetrics", e.Message + e.StackTrace);
        }

        return null;
    }
}