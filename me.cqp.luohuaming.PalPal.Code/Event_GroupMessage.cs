﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using me.cqp.luohuaming.PalPal.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.PalPal.PublicInfos;
using me.cqp.luohuaming.PalPal.PublicInfos.API;
using System.Xml.Linq;

namespace me.cqp.luohuaming.PalPal.Code
{
    public class Event_GroupMessage
    {
        private static readonly Regex img = new Regex("\\[CQ:image.*?\\]");
        private static readonly Regex record = new Regex("\\[CQ:record.*?\\]");
        private static readonly Regex at = new Regex("\\[CQ:at,qq=(\\d*)\\]");
        private static readonly Regex other = new Regex("\\[CQ:.*?\\]");

        private static Dictionary<(long, long), string> MemberCardCache { get; set; } = [];
        private static Dictionary<long, string> GroupNameCache { get; set; } = [];

        public static FunctionResult GroupMessage(CQGroupMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult()
            {
                SendFlag = false
            };
            try
            {
                if (MainSave.EnabledGroup.Contains(e.FromGroup) is false)
                {
                    return result;
                }

                foreach (var item in MainSave.Instances.Where(item => item.Judge(e.Message.Text)))
                {
                    return item.Progress(e);
                }

                if (MainSave.EnableGroupMessageSend && MainSave.GroupSendMessage.Contains(e.FromGroup) && !e.Message.Text.StartsWith(".pal"))
                {
                    Task.Run(() => AnnounceMessage.Announce(FormatMessage(e.FromGroup, e.FromQQ, e.Message.Text)));
                }

                return result;
            }
            catch (Exception exc)
            {
                MainSave.CQLog.Info("异常抛出", exc.Message + exc.StackTrace);
                return result;
            }
        }

        private static string FormatMessage(long group, long qq, string message)
        {
            message = img.Replace(message, "[图片]");
            message = record.Replace(message, "[语音]");
            try
            {
                MatchCollection match = at.Matches(message);
                foreach (Match item in match)
                {
                    if (item.Groups.Count != 2 || long.TryParse(item.Groups[1].Value, out long value))
                    {
                        continue;
                    }

                    if (MemberCardCache.TryGetValue((group, value), out string card) is false)
                    {
                        card = MainSave.CQApi.GetGroupMemberInfo(group, value)?.Card;
                        if (!string.IsNullOrEmpty(card))
                        {
                            MemberCardCache.Add((group, value), card);
                        }
                        else
                        {
                            card = $"@{value}";
                        }
                    }

                    message = message.Replace(item.ToString(), card);
                }

                message = other.Replace(message, "");
                if (string.IsNullOrEmpty(message))
                {
                    return null;
                }
                if (GroupNameCache.TryGetValue(group, out var groupName) is false)
                {
                    groupName = MainSave.CQApi.GetGroupInfo(group)?.Name;
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        GroupNameCache.Add(group, groupName);
                    }
                    else
                    {
                        groupName = group.ToString();
                    }
                }

                if (MemberCardCache.TryGetValue((group, qq), out var name) is false)
                {
                    name = MainSave.CQApi.GetGroupMemberInfo(group, qq)?.Card;
                    if (!string.IsNullOrEmpty(name))
                    {
                        MemberCardCache.Add((group, qq), name);
                    }
                    else
                    {
                        name = qq.ToString();
                    }
                }

                return $"{groupName} {name}: {message}";
            }
            catch
            {
                return "";
            }
        }
    }
}