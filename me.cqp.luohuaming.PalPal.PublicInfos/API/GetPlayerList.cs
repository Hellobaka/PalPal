using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;
using Newtonsoft.Json;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class GetPlayerList
{
    private static readonly string Api = "v1/api/players";

    public static List<PlayerInfo> Get()
    {
        try
        {
            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Get(url);
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            var playerList = JsonConvert.DeserializeObject<PlayerList>(ret);
            if (playerList.players == null)
            {
                throw new HttpRequestException($"Get Result is invalid: {ret}");
            }

            return playerList.players.ToList();
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("GetPlayerList", e.Message + e.StackTrace);
        }

        return null;
    }
}