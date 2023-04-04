/****************************************************
    文件：ClientSession
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年02月27日 星期日 17:12
    功能：客户端网络会话
*****************************************************/

using PEProtocol;
using PESocket;
using PETool.PELogger;
public class ClientSession : PESession {
	override protected void OnConnected() {
		PELogger.Log("服务器连接成功");
	}
	override protected void OnReceiveMsg(byte[] buffer) {
		NetMsg msg = Protocol.Deserialize<NetMsg>(buffer);
		PELogger.Log("Receive Pack CMD: " + msg.Head.Cmd);
		NetSvc.Instance.AddNetPack(msg);
	}

	override protected void OnDisconnected() {
		PELogger.Log("服务器断开连接");
	}
}