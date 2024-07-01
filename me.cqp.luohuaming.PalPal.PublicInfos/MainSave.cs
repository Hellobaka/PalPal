using System;
using me.cqp.luohuaming.PalPal.Sdk.Cqp;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using me.cqp.luohuaming.PalPal.PublicInfos.Models;

namespace me.cqp.luohuaming.PalPal.PublicInfos
{
    public static class MainSave
    {
        /// <summary>
        /// 保存各种事件的数组
        /// </summary>
        public static List<IOrderModel> Instances { get; set; } = new List<IOrderModel>();

        public static CQLog CQLog { get; set; }
        public static CQApi CQApi { get; set; }
        public static string AppDirectory { get; set; }
        public static string ImageDirectory { get; set; }

        public static List<PlayerInfo> PlayerInfos { get; set; } = [];
        
        public static Process PalServerProcess { get; set; }
        
        public static string PalServerUrl { get; set; }

        public static string PalServerPath { get; set; }

        public static string PalServerPassword { get; set; }

        public static bool EnableMemoryMonitor { get; set; }

        public static int MaxMemoryUsage { get; set; }

        public static int ShutDownWaitTime { get; set; }

        public static bool EnableAutoRestart { get; set; }

        public static DateTime AutoRestartTime { get; set; }

        public static List<long> GroupSendMessage { get; set; }

        public static bool EnableGroupMessageSend { get; set; }
        
        public static List<long> EnabledGroup { get; set; }
        
        public static List<long> AdminQQ { get; set; }
    }
}