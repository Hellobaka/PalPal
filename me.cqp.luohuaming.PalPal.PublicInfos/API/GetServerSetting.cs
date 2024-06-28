using System;
using System.Linq;
using System.Net.Http;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;
using Newtonsoft.Json;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class GetServerSetting
{
    private static readonly string Api = "v1/api/settings";

    public static ServerSetting Get()
    {
        try
        {
            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Get(url);
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            var setting = JsonConvert.DeserializeObject<ServerSetting>(ret);
            if (string.IsNullOrEmpty(setting.Difficulty))
            {
                throw new HttpRequestException($"Get Result is invalid: {ret}");
            }

            return setting;
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("GetServerSetting", e.Message + e.StackTrace);
        }

        return null;
    }
}