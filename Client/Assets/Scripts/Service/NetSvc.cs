/****************************************************
    文件：NetService.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/27 17:3:17
    功能：网络服务
*****************************************************/

using PEProtocol;
using PESocket;
using PETool.PELogger;
using System.Collections.Generic;
using UnityEngine;
public class NetSvc : MonoBehaviour {
	public static NetSvc Instance { private set; get; }

	private readonly Queue<NetMsg> msgQueue = new Queue<NetMsg>();
	private PESocket<ClientSession> client;

	public void InitService() {
		Instance = this;
		client = new PESocket<ClientSession>();

		PESocketTool.LogFunc = PELogger.Log;
		PESocketTool.WainFunc = PELogger.Wain;
		PESocketTool.ErrorFunc = PELogger.Error;

		// Debug.Log($"<color=#00FF00> ClientStart </color>");
		client.StartAsClient(Protocol.SeverConfig.SeverIp, Protocol.SeverConfig.SeverPort);
		PELogger.Log("开始连接服务器");
	}

	private void Update() {
		if(msgQueue.Count <= 0) return;
		NetMsg msg = msgQueue.Dequeue();
		ProcessMsg(msg);
	}

	public void SendMessage(NetMsg message) {
		if(client.SendMsg(Protocol.Serialize(message))) return;
		GameRoot.AddTips("服务器未连接...");
		client.Connect();
	}

	public void AddNetPack(NetMsg msg) {
		msgQueue.Enqueue(msg);
	}

	private static void ProcessMsg(NetMsg message) {
		ErrorCode errorCode = message.Head.ErrorCode;

		if(errorCode != ErrorCode.ErrorCodeNone) {
			switch(errorCode) {
				case ErrorCode.ErrorCodeWrongPassword:
					GameRoot.AddTips("密码错误...");
					break;
				case ErrorCode.ErrorCodeAccountIsOnline:
					GameRoot.AddTips("当前账号正在游戏中...");
					break;
				case ErrorCode.ErrorCodeNameExist:
					GameRoot.AddTips("该名称已存在...");
					break;
				case ErrorCode.ErrorCodeUpdataDatabaseError:
					//数据库更新异常
					PELogger.Wain("数据库更新异常");
					GameRoot.AddTips("网络不稳定...");
					break;
				case ErrorCode.ErrorCodeServerDataError:
					PELogger.Wain("服务器数据异常");
					GameRoot.AddTips("网络不稳定...");
					break;
				case ErrorCode.ErrorCodeClientDataError:
					PELogger.Wain("客户端数据异常");
					GameRoot.AddTips("网络不稳定...");
					break;
				case ErrorCode.ErrorCodeLackLevel:
					GameRoot.AddTips("角色等级未达到强化要求...");
					break;
				case ErrorCode.ErrorCodeLackCoin:
					GameRoot.AddTips("剩余金币不足...");
					break;
				case ErrorCode.ErrorCodeLackCrystal:
					GameRoot.AddTips("剩余水晶不足...");
					break;
				case ErrorCode.ErrorCodeLackDiamond:
					GameRoot.AddTips("剩余钻石不足...");
					break;
				case ErrorCode.ErrorCodeLackPower:
					GameRoot.AddTips("剩余体力不足...");
					break;
			}

			return;
		}

		Cmd cmd = message.Head.Cmd;
		
		switch(cmd) {
			case Cmd.CmdResponseLogin:
				LoginSys.Instance.ResponseLogin(message);
				break;
			case Cmd.CmdResponseRename:
				LoginSys.Instance.ResponseRename(message);
				break;
			case Cmd.CmdResponseGuide:
				MainCitySys.Instance.ResponseGuide(message);
				break;
			case Cmd.CmdResponseStrengthen:
				MainCitySys.Instance.ResponseStrengthen(message);
				break;
			case Cmd.CmdPushChat:
				MainCitySys.Instance.PushChat(message);
				break;
			case Cmd.CmdResponseBuy:
				MainCitySys.Instance.ResponseBuy(message);
				break;
			case Cmd.CmdPushPower:
				MainCitySys.Instance.ResponsePower(message);
				break;
			case Cmd.CmdResponseTask:
				MainCitySys.Instance.ResponseTask(message);
				break;
			case Cmd.CmdPushTask:
				MainCitySys.Instance.PushTask(message);
				break;
			case Cmd.CmdResponseDungeon:
				DungeonSys.Instance.ResponseDungeon(message);
				break;
			case Cmd.CmdResponseEndDungeon:
				BattleSys.Instance.ResponseEndDungeon(message);
				break;
		}
	}
}