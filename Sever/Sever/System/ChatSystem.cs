/****************************************************
    文件：ChatSystem
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月14日 星期一 16:09
    功能：聊天系统
*****************************************************/

using System.Collections.Generic;
using PENet;
using PEProtocol;

namespace Sever
{
    public class ChatSystem : SystemRoot
    {
        private static ChatSystem _instance;

        public static ChatSystem Instance => _instance ?? (_instance = new ChatSystem());

        public override void Init()
        {
            base.Init();
            PeCommon.Log("ChatSystem Init Done.");
        }

        public void PushChat(MessagePack messagePack)
        {
            SendChat sendChat = messagePack.Message.SendChat;
            PlayerData playerData = CacheService.Instance.GetPlayerDataBySession(messagePack.Session);

            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.PushChat,
                PushChat = new PushChat
                {
                    Name = playerData.Name,
                    Chat = sendChat.Chat
                }
            };

            TaskSystem.Instance.CheckTaskProgress(playerData, 6);
            //推送到所有在线的客户端
            List<SeverSession> sessionList = CacheService.Instance.GetOnLineSeverSession();
            //避免多次序列化浪费CPU资源(避免三千个客户端,序列化三千遍的情况)
            byte[] messageBytes = PETool.PackNetMsg(message);
            foreach (SeverSession session in sessionList) session.SendMsg(messageBytes);
        }
    }
}