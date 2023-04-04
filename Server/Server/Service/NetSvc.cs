/****************************************************
	文件：NetService.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：网络服务
*****************************************************/

using PEProtocol;
using PESocket;
using PETool.PELogger;

namespace Sever {
	public class NetSvc {
		private const string Obj = "lock";
		private readonly Queue<MessagePack> messagePackQueue = new Queue<MessagePack>();
		public static NetSvc Instance { get; } = new NetSvc();

		public void Init() {
			PESocket<SeverSession> sever = new PESocket<SeverSession>();
			PESocketTool.LogFunc = PELogger.Log;
			PESocketTool.WainFunc = PELogger.Wain;
			PESocketTool.ErrorFunc = PELogger.Error;
			sever.StartAsServer(Protocol.SeverConfig.SeverIp, Protocol.SeverConfig.SeverPort, 100);
		}

		public void AddMsgQueue(MessagePack messagePack) {
			lock(Obj) {
				messagePackQueue.Enqueue(messagePack);
			}
		}

		public void Updata() {
			lock(Obj) {
				if(messagePackQueue.Count <= 0) return;
				MessagePack messagePack = messagePackQueue.Dequeue();
				HandOutMessage(messagePack);
			}
		}

		private static void HandOutMessage(MessagePack messagePack) {
			switch(messagePack.message.Head.Cmd) {
				case Cmd.CmdRequestLogin:
					LoginSystem.Instance.ResponseLogin(messagePack);
					break;
				case Cmd.CmdRequestRename:
					LoginSystem.Instance.ResponseRename(messagePack);
					break;
				case Cmd.CmdRequestGuide:
					GuideSystem.Instance.ResponseGuide(messagePack);
					break;
				case Cmd.CmdRequestStrengthen:
					StrengthenSystem.Instance.ResponseStrengthen(messagePack);
					break;
				case Cmd.CmdSendChat:
					ChatSystem.Instance.PushChat(messagePack);
					break;
				case Cmd.CmdRequestBuy:
					BuySystem.Instance.ResponseBuy(messagePack);
					break;
				case Cmd.CmdRequestTask:
					TaskSystem.Instance.ResponseTask(messagePack);
					break;
				case Cmd.CmdRequestDungeon:
					DungeonSystem.Instance.ResponseDungeon(messagePack);
					break;
				case Cmd.CmdRequestEndDungeon:
					DungeonSystem.Instance.ResponseEndDungeon(messagePack);
					break;
			}
		}
	}
}