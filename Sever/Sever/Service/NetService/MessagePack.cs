/****************************************************
	文件：MessagePack.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：网络消息包
*****************************************************/

using PEProtocol;

namespace Sever
{
    public class MessagePack
    {
        public readonly GameMessage Message;
        public readonly SeverSession Session;

        public MessagePack(SeverSession session, GameMessage message)
        {
            Session = session;
            Message = message;
        }
    }
}