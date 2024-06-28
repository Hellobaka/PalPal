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

namespace me.cqp.luohuaming.PalPal.Code.OrderFunctions
{
    public class Unban : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => ".pal unban";

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
            
            string steamId = e.Message.Text.Replace(GetOrderStr(), "").Trim();
            if (string.IsNullOrEmpty(steamId))
            {
                sendText.MsgToSend.Add("无效指令，请添加需要提出的玩家SteamID");
                return result;
            }

            if (PublicInfos.API.UnbanPlayer.Unban(steamId) is false)
            {
                sendText.MsgToSend.Add("向服务器发送指令失败");
                return result;
            }

            sendText.MsgToSend.Add($"玩家 {steamId} 已被解除Ban");
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