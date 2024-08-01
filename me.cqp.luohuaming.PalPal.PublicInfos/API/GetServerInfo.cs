using System;
using System.Diagnostics;
using System.Net.Http;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;
using Newtonsoft.Json;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API
{
    public class GetServerInfo
    {
        private static readonly string Api = "v1/api/info";

        public static ServerInfo Get()
        {
            try
            {
                Process p = CommonHelper.GetOrFindProcess();
                if (p == null || p.HasExited)
                {
                    return null;
                }

                string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
                string ret = CommonHelper.Get(url);
                if (string.IsNullOrEmpty(ret))
                {
                    throw new HttpRequestException("Get Result is invalid");
                }

                var serverinfo = JsonConvert.DeserializeObject<ServerInfo>(ret);
                if (string.IsNullOrEmpty(serverinfo.version))
                {
                    throw new HttpRequestException($"Get Result is invalid: {ret}");
                }

                return serverinfo;
            }
            catch (Exception e)
            {
                MainSave.CQLog.Error("GetServerInfo", e.Message + e.StackTrace);
            }

            return null;
        }
    }
}