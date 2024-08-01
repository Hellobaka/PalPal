﻿using System;
using System.Diagnostics;
using System.Net.Http;

namespace me.cqp.luohuaming.PalPal.PublicInfos.API;

public class BanPlayer
{
    private static readonly string Api = "v1/api/ban";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userid">steamid</param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public static bool Ban(string userid, string message)
    {
        try
        {
            Process p = CommonHelper.GetOrFindProcess();
            if (p == null || p.HasExited)
            {
                return false;
            }

            string url = CommonHelper.CombineUrl(MainSave.PalServerUrl, Api);
            string ret = CommonHelper.Post(url, new{ userid, message });
            if (string.IsNullOrEmpty(ret))
            {
                throw new HttpRequestException("Get Result is invalid");
            }

            return !string.IsNullOrEmpty(ret);
        }
        catch (Exception e)
        {
            MainSave.CQLog.Error("BanPlayer", e.Message + e.StackTrace);
        }

        return false;
    }
}