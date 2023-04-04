/****************************************************
    文件：ChatSystem
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月14日 星期一 16:09
    功能：聊天系统
*****************************************************/

using PEProtocol;
using PETool.PELogger;

namespace Sever {
	public class ChatSystem : SystemRoot<ChatSystem> {
		public override void Init() {
			base.Init();
			PELogger.Log("ChatSystem Init Done.");
		}

		public void PushChat(MessagePack messagePack) {
			SendChat sendChat = messagePack.message.Body.SendChat;
			PlayerData? playerData = CacheSvc.Instance.GetPlayerDataBySession(messagePack.session);

			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdPushChat
				},
				Body = new Body{
					PushChat = new PushChat{
						Name = playerData?.Name,
						Chat = sendChat.Chat
					}
				}
			};

			TaskSystem.Instance.CheckTaskSingle(playerData!, TaskType.Chat);
			//推送到所有在线的客户端
			List<SeverSession> sessionList = CacheSvc.Instance.GetOnLineSeverSession();
			byte[] messageBytes = Protocol.Serialize(message);

			foreach(SeverSession session in sessionList) session.SendMsg(messageBytes);
		}
	}
}