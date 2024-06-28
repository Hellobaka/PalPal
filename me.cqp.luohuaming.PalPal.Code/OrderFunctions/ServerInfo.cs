using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.PalPal.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.PalPal.PublicInfos;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;
using me.cqp.luohuaming.PalPal.PublicInfos.API;

namespace me.cqp.luohuaming.PalPal.Code.OrderFunctions
{
    public class ServerInfo : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => ".pal info";

        public bool Judge(string destStr) => destStr.Replace("＃", "#").StartsWith(GetOrderStr());

        public FunctionResult Progress(CQGroupMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            result.SendObject.Add(sendText);

            var info1 = GetServerMetrics.Get();
            var info2 = GetServerInfo.Get();
            var p = CommonHelper.GetOrFindProcess();
            if (info1 != null && info2 != null && p != null && !p.HasExited) 
            {
                var timeSpan = TimeSpan.FromSeconds(info1.uptime);
                sendText.MsgToSend.Add($"服务器信息：\n" +
                    $"名称：{info2.servername}\n" +
                    $"描述：{info2.description}\n" +
                    $"人数：{info1.currentplayernum}/{info1.maxplayernum}\n" +
                    $"服务器帧数：{info1.serverfps}fps\n" +
                    $"启动时间：{(int)timeSpan.TotalDays} 天 {timeSpan.Hours} 小时 {timeSpan.Minutes} 分钟\n" +
                    $"占用内存：{p.PrivateMemorySize64 / 1024.0 / 1024 / 1024:f2}GB");
                return result;
            }
            else
            {
                sendText.MsgToSend.Add("无法从服务器获取信息");
                return result;
            }
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)
        {
            return new FunctionResult
            {
                Result = false,
                SendFlag = false,
            };
        }
    }
}