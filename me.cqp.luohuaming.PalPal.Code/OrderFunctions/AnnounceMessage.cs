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
    public class AnnounceMessage : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => ".pal send";

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

            string msg = e.Message.Text.Replace(GetOrderStr(), "").Trim();
            if (string.IsNullOrEmpty(msg))
            {
                sendText.MsgToSend.Add("无效指令，请添加需要发送的文本");
                return result;
            }

            if (PublicInfos.API.AnnounceMessage.Announce(msg) is false)
            {
                sendText.MsgToSend.Add("向服务器发送指令失败");
                return result;
            }

            result.SendFlag = false;
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