/****************************************************
	文件：MessagePack.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：网络消息包
*****************************************************/

using PEProtocol;

namespace Sever {
	public class MessagePack {
		public readonly NetMsg message;
		public readonly SeverSession session;

		public MessagePack(SeverSession session, NetMsg message) {
			this.session = session;
			this.message = message;
		}
	}
}