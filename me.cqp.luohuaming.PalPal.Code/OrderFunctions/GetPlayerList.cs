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
    public class GetPlayerList : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => ".pal list";

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

            var list = PublicInfos.API.GetPlayerList.Get();
            if (list == null)
            {
                sendText.MsgToSend.Add("向服务器发送请求失败");
                return result;
            }

            MainSave.PlayerInfos = list;
            StringBuilder sb = new();
            for (var i = 0; i < list.Count - 1; i++)
            {
                var item = list[i];
                sb.AppendLine($"{i + 1}. {item.name}(Lv.{item.level})[{item.userId}]({(int)item.ping}ms)");
            }

            if (list.Count > 0)
            {
                var item = list.Last();
                sb.Append($"{list.Count}. {item.name}(Lv.{item.level})[{item.userId}]({(int)item.ping}ms)");
            }

            sendText.MsgToSend.Add($"玩家列表[{list.Count}]：\n" + sb);
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