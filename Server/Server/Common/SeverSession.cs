/****************************************************
	文件：SeverSession.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022/02/27 15:08   	
	功能：网络回话连接
*****************************************************/

using PESocket;
using PETool.PELogger;
using PEProtocol;

namespace Sever {
	public class SeverSession : PESession {
		public int sessionId;

		override protected void OnConnected() {
			sessionId = SeverRoot.Instance.GetSessionId();
			PELogger.Log($"SessionID: {sessionId.ToString()} Client Connected...");
		}
		override protected void OnReceiveMsg(byte[] buffer) {
			NetMsg message = Protocol.Deserialize<NetMsg>(buffer);
			PELogger.Log($"SessionID: {sessionId.ToString()} ReceivePack Cmd: {message.Head.Cmd}");
			NetSvc.Instance.AddMsgQueue(new MessagePack(this, message));
		}
		override protected void OnDisconnected() {
			LoginSystem.Instance.ClearOfflineData(this);
			PELogger.Log($"SessionID: {sessionId.ToString()} Client Offline...");
		}
	}
}