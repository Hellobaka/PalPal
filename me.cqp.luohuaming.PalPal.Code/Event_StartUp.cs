using me.cqp.luohuaming.PalPal.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.PalPal.Sdk.Cqp.Interface;
using me.cqp.luohuaming.PalPal.PublicInfos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;
using Timer = System.Timers.Timer;

namespace me.cqp.luohuaming.PalPal.Code
{
    public class Event_StartUp : ICQStartup
    {
        public void CQStartup(object sender, CQStartupEventArgs e)
        {
            MainSave.AppDirectory = e.CQApi.AppDirectory;
            MainSave.CQApi = e.CQApi;
            MainSave.CQLog = e.CQLog;
            MainSave.ImageDirectory = CommonHelper.GetAppImageDirectory();
            ConfigHelper.ConfigFileName = Path.Combine(MainSave.AppDirectory, "Config.json");
            if (ConfigHelper.Load() is false)
            {
                MainSave.CQLog.Warning("加载配置文件", "内容格式不正确，无法加载");
            }

            foreach (var item in Assembly.GetAssembly(typeof(Event_GroupMessage)).GetTypes())
            {
                if (item.IsInterface)
                    continue;
                foreach (var instance in item.GetInterfaces())
                {
                    if (instance == typeof(IOrderModel))
                    {
                        IOrderModel obj = (IOrderModel)Activator.CreateInstance(item);
                        if (obj.ImplementFlag == false)
                            break;
                        MainSave.Instances.Add(obj);
                    }
                }
            }

            LoadConfig();
            FileSystemWatcher configChangeWatcher = new FileSystemWatcher();
            configChangeWatcher.Path = Path.GetDirectoryName(ConfigHelper.ConfigFileName);
            configChangeWatcher.Filter = Path.GetFileName(ConfigHelper.ConfigFileName);
            configChangeWatcher.NotifyFilter = NotifyFilters.LastWrite;
            configChangeWatcher.Changed += ConfigChangeWatcher_Changed;
            configChangeWatcher.EnableRaisingEvents = true;

            MemoryUsageMonitor = new Timer();
            MemoryUsageMonitor.Interval = 1000;
            MemoryUsageMonitor.Elapsed += MemoryUsageMonitor_Elasped;
            MemoryUsageMonitor.Enabled = true;
            MemoryUsageMonitor.Start();
        }

        private bool TimerExecuting { get; set; }

        private void MemoryUsageMonitor_Elasped(object sender, ElapsedEventArgs e)
        {
            if (MainSave.EnableMemoryMonitor is false || TimerExecuting)
            {
                return;
            }

            TimerExecuting = true;
            try
            {
                Process p = CommonHelper.GetOrFindProcess();
                if (p == null || p.HasExited)
                {
                    return;
                }

                var memory = p.PrivateMemorySize64 / 1024.0 / 1024;
                bool restart = false;
                if (memory > MainSave.MaxMemoryUsage)
                {
                    restart = true;
                    MainSave.CQLog.Info("服务内存超载", $"当前内存: {memory:f2}MB");
                    MemoryUsageMonitor.Stop();
                    p = CommonHelper.RestartServer(p, "服务内存超载，自动重启");
                    if (p != null) 
                    {
                        MainSave.PalServerProcess = p;
                        MainSave.CQLog.Info("服务内存超载", $"重启完成");
                        MemoryUsageMonitor.Start();
                    }
                    else
                    {
                        MainSave.CQLog.Warning("服务内存超载", $"重启失败，可能由于路径错误。内存检查时钟已停止，请重载插件以恢复");
                    }
                }

                var datetime = DateTime.Now;
                var autoRestart = MainSave.AutoRestartTime;
                if (datetime.Hour == autoRestart.Hour && datetime.Minute == autoRestart.Minute &&
                    datetime.Second == autoRestart.Second
                    && restart is false)
                {
                    MainSave.CQLog.Info("定时重启", $"当前内存: {memory:f2}MB");
                    MemoryUsageMonitor.Stop();

                    p = CommonHelper.RestartServer(p, "定时重启");
                    if (p != null)
                    {
                        MainSave.PalServerProcess = CommonHelper.RestartServer(p, "定时重启");

                        MainSave.CQLog.Info("定时重启", $"重启完成");
                        MemoryUsageMonitor.Start();
                    }
                    else
                    {
                        MainSave.CQLog.Warning("定时重启", $"重启失败，可能由于路径错误。内存检查时钟已停止，请重载插件以恢复");
                    }
                }
            }
            catch
            {
            }
            finally
            {
                TimerExecuting = false;
            }
        }

        public Timer MemoryUsageMonitor { get; set; }

        private void ConfigChangeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Changed && ConfigHelper.Load())
                {
                    LoadConfig();
                }
            }
            catch
            {
            }
        }

        private void LoadConfig()
        {
            MainSave.PalServerUrl = ConfigHelper.GetConfig("PalServerUrl", "http://127.0.0.1:8212");
            MainSave.PalServerPath = ConfigHelper.GetConfig("PalServerPath", "");
            MainSave.PalServerPassword = ConfigHelper.GetConfig("PalServerPassword", "123456");

            MainSave.AutoRestartTime = ConfigHelper.GetConfig("AutoRestartTime", new DateTime());
            MainSave.EnableAutoRestart = ConfigHelper.GetConfig("EnableAutoRestart", false);
            MainSave.EnableMemoryMonitor = ConfigHelper.GetConfig("EnableMemoryMonitor", false);
            MainSave.GroupSendMessage = ConfigHelper.GetConfig("GroupSendMessage", new List<long>());
            MainSave.EnabledGroup = ConfigHelper.GetConfig("EnabledGroup", new List<long>());
            MainSave.AdminQQ = ConfigHelper.GetConfig("AdminQQ", new List<long>());
            MainSave.MaxMemoryUsage = ConfigHelper.GetConfig("MaxMemoryUsage", 10 * 1024);
            MainSave.EnableGroupMessageSend = ConfigHelper.GetConfig("EnableGroupMessageSend", false);
            MainSave.ShutDownWaitTime = ConfigHelper.GetConfig("ShutDownWaitTime", 10);
        }
    }
}