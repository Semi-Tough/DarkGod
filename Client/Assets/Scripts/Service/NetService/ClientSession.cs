/****************************************************
    文件：ClientSession
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年02月27日 星期日 17:12
    功能：客户端网络会话
*****************************************************/

using PENet;
using PEProtocol;

public class ClientSession : PESession<GameMessage>
{
    protected override void OnConnected()
    {
        PeCommon.Log("服务器连接成功");
    }

    protected override void OnReciveMsg(GameMessage msg)
    {
        PeCommon.Log("Receive Pack CMD: " + (Cmd) msg.cmd);
        NetService.instance.AddNetPack(msg);
    }

    protected override void OnDisConnected()
    {
        PeCommon.Log("服务器断开连接");
    }
}