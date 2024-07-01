using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using me.cqp.luohuaming.PalPal.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.PalPal.PublicInfos;
using me.cqp.luohuaming.PalPal.PublicInfos.API;

namespace me.cqp.luohuaming.PalPal.Code.OrderFunctions
{
    public class RestartServer : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => ".pal restart";

        public bool Judge(string destStr) => destStr.Replace("＃", "#").StartsWith(GetOrderStr());

        public FunctionResult Progress(CQGroupMessageEventArgs e)
        {
            if (!MainSave.AdminQQ.Contains(e.FromQQ))
            {
                return new FunctionResult
                {
                    Result = false,
                    SendFlag = false,
                };
            }
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

            if (File.Exists(MainSave.PalServerPath) is false)
            {
                sendText.MsgToSend.Add("指定的服务文件路径不存在，无法重启");
                return result;
            }
            e.FromGroup.SendGroupMessage($"已向服务器广播停止消息，请等待");
            var p = CommonHelper.RestartServer(MainSave.PalServerProcess, "管理员主动重启服务器");
            if (p != null)
            {
                sendText.MsgToSend.Add("服务已重新启动");
                return result;
            }
            else
            {
                sendText.MsgToSend.Add("服务重启失败");
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
