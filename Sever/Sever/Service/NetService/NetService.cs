/****************************************************
	文件：NetService.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：网络服务
*****************************************************/

using System.Collections.Generic;
using PENet;
using PEProtocol;

namespace Sever
{
    public class NetService
    {
        private const string Obj = "lock";
        private static NetService _instance;

        private readonly Queue<MessagePack> _messagePackQueue = new Queue<MessagePack>();
        public static NetService Instance => _instance ?? (_instance = new NetService());

        public void Init()
        {
            PESocket<SeverSession, GameMessage> sever = new PESocket<SeverSession, GameMessage>();
            sever.StartAsServer(SeverConfig.SeverIp, SeverConfig.SeverPort);
            PeCommon.Log("NetService Init Done.");
        }

        public void AddMsgQueue(MessagePack messagePack)
        {
            lock (Obj)
            {
                _messagePackQueue.Enqueue(messagePack);
            }
        }

        public void UpData()
        {
            lock (Obj)
            {
                if (_messagePackQueue.Count <= 0) return;
                PeCommon.Log("PackCount: " + _messagePackQueue.Count);
                MessagePack messagePack = _messagePackQueue.Dequeue();
                HandOutMessage(messagePack);
            }
        }

        private static void HandOutMessage(MessagePack messagePack)
        {
            switch ((Cmd) messagePack.Message.cmd)
            {
                case Cmd.RequestLogin:
                    LoginSystem.Instance.ResponseLogin(messagePack);
                    break;
                case Cmd.RequestRename:
                    LoginSystem.Instance.ResponseRename(messagePack);
                    break;
                case Cmd.RequestGuide:
                    GuideSystem.Instance.ResponseGuide(messagePack);
                    break;
                case Cmd.RequestStrengthen:
                    StrengthenSystem.Instance.ResponseStrengthen(messagePack);
                    break;
                case Cmd.SendChat:
                    ChatSystem.Instance.PushChat(messagePack);
                    break;
                case Cmd.RequestBuy:
                    BuySystem.Instance.ResponseBuy(messagePack);
                    break;
                case Cmd.RequestTask:
                    TaskSystem.Instance.ResponseTask(messagePack);
                    break;
                case Cmd.RequestDungeon:
                    DungeonSystem.Instance.ResponseDungeon(messagePack);
                    break;
            }
        }
    }
}