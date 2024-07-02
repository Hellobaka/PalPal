# PalPal
帕鲁帕鲁~
帕鲁专有服务器管理
- 定时重启
- 内存超限重启
- 转发群消息至游戏内广播
- 获取服务器内玩家列表
- 获取服务器状态
- 指令踢出/Ban玩家
- 指令启动、关闭、重启服务器

此插件只能部署在与服务器相同的机器上，因为需要控制进程的启停
需要在服务器中开启rest服务器`RESTAPIEnabled=True,RESTAPIPort=8212`

## 配置
| 配置键     | 含义     | 默认值     |
| -------- | -------- | -------- |
| PalServerUrl | 连接帕鲁专有服务器Restful接口的url | http://127.0.0.1:8212 |
| PalServerPath | 服务器在本地的路径 |  |
| PalServerPassword | 服务器中配置的AdminPassword | 123456 |
| AutoRestartTime | 每日自动重启时间，请修改时间部分 | 0001-01-01T00:00:00 |
| EnableAutoRestart | 是否每日执行自动重启 | false |
| EnableMemoryMonitor | 是否启动内存占用监控 | false |
| GroupSendMessage | 接受广播的群号 | [] |
| EnabledGroup | 能够使用指令的群 | [] |
| AdminQQ | 管理员QQ | [] |
| MaxMemoryUsage | 服务器内存最大占用（MB），超过时重启 | 10240 |
| EnableGroupMessageSend | 启用群消息转发 | false |
| ShutDownWaitTime | 所有关闭服务器操作的等待时长（秒） | 10 |

## 指令
| 指令     | 作用 | 权限    | 示例     |
| -------- | -------- | -------- | -------- |
|.pal send [消息]|向游戏内广播消息|无|.pal send 啊啊啊|
|.pal ban [列表序号/SteamID/名称]|Ban指定用户|Admin|.pal ban 1/steam_xxxx|
|.pal list|获取服务器内玩家列表|无|.pal list|
|.pal kick [列表序号/SteamID/名称]|从服务器中踢出指定玩家|Admin|.pal kick 1/steam_xxxx|
|.pal restart|重启服务器|Admin|.pal restart|
|.pal info|获取服务器状态|无|.pal info|
|.pal start|启动服务器|Admin|.pal start|
|.pal stop|关闭服务器|Admin|.pal stop|
|.pal unban [SteamID]|解除玩家的ban|Admin|.pal unban steam_xxxx|

## 内容
- info
```
服务器信息：
名称：Boy Next♂Door
描述：Ass♂We Can
人数：0/32
服务器帧数：59fps
启动时间：0 天 15 小时 4 分钟
占用内存：3.27GB
```

- list
```
玩家列表[3]：
1. 真新镇小智[steam_765xxxxxxxx14](64ms)
2. 叔君[steam_765xxxxxxxxx06](94ms)
3. 落花茗[steam_765xxxxxxxxx01](16ms)
```