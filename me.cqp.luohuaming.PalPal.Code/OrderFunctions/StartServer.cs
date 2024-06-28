using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.PalPal.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.PalPal.PublicInfos;

namespace me.cqp.luohuaming.PalPal.Code.OrderFunctions
{
    public class StartServer : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;
        
        public string GetOrderStr() => ".pal start";

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

            if (p != null && p.HasExited is false)
            {
                sendText.MsgToSend.Add("服务已启动，请勿重复启动");
                return result;
            }

            if (File.Exists(MainSave.PalServerPath) is false)
            {
                sendText.MsgToSend.Add("指定的服务文件路径不存在");
                return result;
            }

            MainSave.PalServerProcess = Process.Start(new ProcessStartInfo
            {
                FileName = MainSave.PalServerPath,
                WorkingDirectory = Path.GetDirectoryName(MainSave.PalServerPath)
            });
            
            sendText.MsgToSend.Add("服务已启动");
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
