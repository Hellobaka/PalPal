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
    public class Kick : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => ".pal kick";

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
            
            string target = e.Message.Text.Replace(GetOrderStr(), "").Trim();
            if (string.IsNullOrEmpty(target))
            {
                sendText.MsgToSend.Add("无效指令，请添加需要踢出的玩家序号或名称或SteamID");
                return result;
            }

            int index = int.TryParse(target, out int v) ? v : -1;
            string steamId = "";
            PlayerInfo info = null;
            index--;
            if (index > 0 && index < MainSave.PlayerInfos.Count)
            {
                info = MainSave.PlayerInfos[index];
            }
            else
            {
                info = MainSave.PlayerInfos.FirstOrDefault(x =>
                    x.name == target || x.accountName == target || x.userId == target);
            }
            steamId = info?.userId;
            
            if (string.IsNullOrEmpty(steamId))
            {
                sendText.MsgToSend.Add("未找到指定的玩家，尝试使用完整条件限制");
                return result;
            }

            if (PublicInfos.API.KickPlayer.Kick(steamId, "管理员将您踢出了游戏") is false)
            {
                sendText.MsgToSend.Add("向服务器发送指令失败");
                return result;
            }

            sendText.MsgToSend.Add($"玩家 {info.accountName}[{info.userId}] 已被踢出");
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