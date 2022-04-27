/****************************************************
	文件：SeverSession.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/02/27 15:08   	
	功能：网络回话连接
*****************************************************/

using PENet;
using PEProtocol;

namespace Sever
{
    public class SeverSession : PESession<GameMessage>
    {
        public int SessionId;

        protected override void OnConnected()
        {
            SessionId = SeverRoot.Instance.GetSessionId();
            PeCommon.Log("SessionID: " + SessionId + " Client Connected...");
        }

        protected override void OnReciveMsg(GameMessage message)
        {
            PeCommon.Log("SessionID: " + SessionId + " ReceivePack Cmd: " + (Cmd) message.cmd);
            NetService.Instance.AddMsgQueue(new MessagePack(this, message));
        }

        protected override void OnDisConnected()
        {
            LoginSystem.Instance.ClearOfflineData(this);
            PeCommon.Log("SessionID: " + SessionId + " Client Offline...");
        }
    }
}