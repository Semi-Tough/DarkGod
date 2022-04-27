/****************************************************
    文件：NetService.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/27 17:3:17
    功能：网络服务
*****************************************************/

using System.Collections.Generic;
using PENet;
using PEProtocol;
using UnityEngine;

public class NetService : MonoBehaviour
{
    public static NetService instance;

    private readonly Queue<GameMessage> _msgQueue = new Queue<GameMessage>();
    private PESocket<ClientSession, GameMessage> _client;

    public void InitService()
    {
        instance = this;
        _client = new PESocket<ClientSession, GameMessage>();
        _client.SetLog(true, (message, level) =>
        {
            switch (level)
            {
                case 0:
                    message = "Log: " + message;
                    Debug.Log(message);
                    break;
                case 1:
                    message = "Warn: " + message;
                    Debug.LogWarning(message);
                    break;
                case 2:
                    message = "Error: " + message;
                    Debug.LogError(message);
                    break;
                case 3:
                    message = "Info: " + message;
                    Debug.Log(message);
                    break;
            }
        });
        _client.StartAsClient(SeverConfig.SeverIp, SeverConfig.SeverPort);
        PeCommon.Log("开始连接服务器");
    }

    private void Update()
    {
        if (_msgQueue.Count <= 0) return;
        GameMessage msg = _msgQueue.Dequeue();
        ProcessMsg(msg);
    }

    public void SendMessage(GameMessage message)
    {
        if (_client.session != null)
        {
            _client.session.SendMsg(message);
        }
        else
        {
            GameRoot.AddTips("服务器未连接...");
            InitService();
        }
    }

    public void AddNetPack(GameMessage msg)
    {
        _msgQueue.Enqueue(msg);
    }

    private static void ProcessMsg(GameMessage message)
    {
        if (message.err != (int) ErrorCode.None)
        {
            switch ((ErrorCode) message.err)
            {
                case ErrorCode.WrongPassword:
                    GameRoot.AddTips("密码错误...");
                    break;
                case ErrorCode.AccountIsOnline:
                    GameRoot.AddTips("当前账号正在游戏中...");
                    break;
                case ErrorCode.UpDataDataBaseError:
                    //数据库更新异常
                    PeCommon.Log("数据库更新异常", PeLogType.Error);
                    GameRoot.AddTips("网络不稳定...");
                    break;
                case ErrorCode.SeverDateError:
                    PeCommon.Log("服务器数据异常", PeLogType.Error);
                    GameRoot.AddTips("网络不稳定...");
                    break;
                case ErrorCode.ClientDataError:
                    PeCommon.Log("客户端数据异常", PeLogType.Error);
                    GameRoot.AddTips("网络不稳定...");
                    break;
                case ErrorCode.LackLevel:
                    GameRoot.AddTips("角色等级未达到强化要求...");
                    break;
                case ErrorCode.LackCoin:
                    GameRoot.AddTips("剩余金币不足...");
                    break;
                case ErrorCode.LackCrystal:
                    GameRoot.AddTips("剩余水晶不足...");
                    break;
                case ErrorCode.LackDiamond:
                    GameRoot.AddTips("剩余钻石不足...");
                    break;
                case ErrorCode.LackPower:
                    GameRoot.AddTips("剩余体力不足...");
                    break;
            }

            return;
        }


        switch ((Cmd) message.cmd)
        {
            case Cmd.ResponseLogin:
                LoginSystem.Instance.ResponseLogin(message);
                break;
            case Cmd.ResponseRename:
                LoginSystem.Instance.ResponseRename(message);
                break;
            case Cmd.ResponseGuide:
                MainCitySystem.Instance.ResponseGuide(message);
                break;
            case Cmd.ResponseStrengthen:
                MainCitySystem.Instance.ResponseStrengthen(message);
                break;
            case Cmd.PushChat:
                MainCitySystem.Instance.PushChat(message);
                break;
            case Cmd.ResponseBuy:
                MainCitySystem.Instance.ResponseBuy(message);
                break;
            case Cmd.PushPower:
                MainCitySystem.Instance.ResponsePower(message);
                break;
            case Cmd.ResponseTask:
                MainCitySystem.Instance.ResponseTask(message);
                break;
            case Cmd.PushTask:
                MainCitySystem.Instance.PushTask(message);
                break;
            case Cmd.ResponseDungeon:
                DungeonSystem.Instance.ResponseDungeon(message);
                break;
        }
    }
}