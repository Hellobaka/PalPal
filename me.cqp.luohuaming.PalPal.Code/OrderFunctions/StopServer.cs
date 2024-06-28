using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.PalPal.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.PalPal.PublicInfos;
using me.cqp.luohuaming.PalPal.PublicInfos.API;

namespace me.cqp.luohuaming.PalPal.Code.OrderFunctions
{
    public class StopServer : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => ".pal stop";

        public bool Judge(string destStr) => destStr.Replace("＃", "#").StartsWith(GetOrderStr());

        public FunctionResult Progress(CQGroupMessageEventArgs e)
        {
            if (e.FromQQ != MainSave.AdminQQ)
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
            var p = CommonHelper.GetOrFindProcess();
            if (p == null || p.HasExited)
            {
                sendText.MsgToSend.Add("无法找到目标进程");
                return result;
            }
            
            try
            {
                bool sendShutdown = ShutDownServer.ShutDown(MainSave.ShutDownWaitTime, "管理员主动关闭服务器");
                if (sendShutdown)
                {
                    e.FromGroup.SendGroupMessage("已向服务器广播关闭消息，等待进程关闭...");
                }
                if (Task.Run(() => p?.WaitForExit()).Wait((MainSave.ShutDownWaitTime + 5) * 1000) is false)
                {
                    p?.Kill();
                }
                sendText.MsgToSend.Add("服务已终止");
            }
            catch (Exception exc)
            {
                sendText.MsgToSend.Add("服务终止失败");
                e.CQLog.Error("StopServer", e.Message);
            }
            
            return result;
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