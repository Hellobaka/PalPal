using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using me.cqp.luohuaming.PalPal.PublicInfos.API;
using me.cqp.luohuaming.PalPal.Sdk.Cqp.Model;

namespace me.cqp.luohuaming.PalPal.PublicInfos
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static string GetAppImageDirectory()
        {
            var ImageDirectory = Path.Combine(Environment.CurrentDirectory, "data", "image\\");
            return ImageDirectory;
        }

        public static string ParseLongNumber(int num)
        {
            string numStr = num.ToString();
            int step = 1;
            for (int i = numStr.Length - 1; i > 0; i--)
            {
                if (step % 3 == 0)
                {
                    numStr = numStr.Insert(i, ",");
                }
                step++;
            }
            return numStr;
        }

        public static string GetFileNameFromURL(this string url)
        {
            return url.Split('/').Last().Split('?').First();
        }

        public static string ParseNum2Chinese(this int num)
        {
            return num > 10000 ? $"{num / 10000.0:f1}万" : num.ToString();
        }

        public static bool CompareNumString(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return a.Length > b.Length;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return a[i] > b[i];
                }
            }
            return false;
        }

        private static string GetBasicAuth() => Convert.ToBase64String(Encoding.UTF8.GetBytes($"admin:{MainSave.PalServerPassword}"));

        public static string Get(string url, int timeout = 5000)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", GetBasicAuth());
                
                var getTask = httpClient.GetAsync(url);
                bool waitResult = getTask.Wait(timeout);

                if (waitResult is false)
                {
                    throw new TimeoutException();
                }
                getTask.Result.EnsureSuccessStatusCode();
                var readTask = getTask.Result.Content.ReadAsStringAsync();
                readTask.Wait();
                return readTask.Result;
            }
            catch (Exception e)
            {
                MainSave.CQLog.Warning("Get", $"请求 {url} 过程发生异常：" + e.Message + e.StackTrace);
            }

            return "";
        }

        public static string Post(string url, object body, int timeout = 5000)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", GetBasicAuth());
                
                var postTask = httpClient.PostAsync(url, JsonContent.Create(body));
                bool waitResult = postTask.Wait(timeout);

                if (waitResult is false)
                {
                    throw new TimeoutException();
                }
                postTask.Result.EnsureSuccessStatusCode();
                var readTask = postTask.Result.Content.ReadAsStringAsync();
                readTask.Wait();
                return readTask.Result;
            }
            catch (Exception e)
            {
                MainSave.CQLog.Warning("Post", $"请求 {url} 过程发生异常：" + e.Message + e.StackTrace);
            }

            return "";
        }

        public static string CombineUrl(string url, string url2)
        {
            if (!url.EndsWith("/"))
            {
                url = url + "/";
            }
            if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }
            return url + url2;
        }

        public static Process RestartServer(Process p, string message)
        {
            if (!File.Exists(MainSave.PalServerPath))
            {
                return null;
            }
            ShutDownServer.ShutDown(MainSave.ShutDownWaitTime, message);
            if (Task.Run(() => p?.WaitForExit()).Wait((MainSave.ShutDownWaitTime + 5) * 1000) is false)
            {
                p?.Kill();
            }
            
            Thread.Sleep(500);
            return Process.Start(new ProcessStartInfo
            {
                FileName = MainSave.PalServerPath,
                WorkingDirectory = Path.GetDirectoryName(MainSave.PalServerPath)
            });
        }

        public static Process GetOrFindProcess()
        {
            Process p = MainSave.PalServerProcess;
            if (p == null || p.HasExited)
            {
                string processName = "PalServer-Win64-Shipping-Cmd";
                p = Process.GetProcessesByName(processName).FirstOrDefault();
                if (p == null && File.Exists(MainSave.PalServerPath))
                {
                    processName = Path.GetFileName(MainSave.PalServerPath);
                    p = Process.GetProcessesByName(processName).FirstOrDefault();
                }

                MainSave.PalServerProcess = p;
            }

            if (p == null || p.HasExited)
            {
                return p;
            }
            p.Refresh();
            
            return p;
        }
    }
}
