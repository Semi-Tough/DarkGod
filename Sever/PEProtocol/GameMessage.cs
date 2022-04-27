/****************************************************
	文件：Class1.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/02/27 14:47   	
	功能：网络通讯协议(客户端和服务端工业)
*****************************************************/

using System;
using PENet;

namespace PEProtocol
{
    [Serializable]
    public class GameMessage : PEMsg
    {
        public PushChat PushChat;
        public PushPower PushPower;
        public PushTask PushTask;

        public RequestBuy RequestBuy;
        public RequestGuide RequestGuide;
        public RequestLogin RequestLogin;
        public RequestRename RequestRename;
        public RequestStrengthen RequestStrengthen;
        public RequestTask RequestTask;
        public RequestDungeon RequestDungeon;

        public ResponseBuy ResponseBuy;
        public ResponseGuide ResponseGuide;
        public ResponseLogin ResponseLogin;
        public ResponseRename ResponseRename;
        public ResponseStrengthen ResponseStrengthen;
        public ResponseTask ResponseTask;
        public ResponseDungeon ResponseDungeon;

        public SendChat SendChat;
    }

    #region 属性相关

    [Serializable]
    public class PlayerData
    {
        public int Ad;
        public int AdDef;
        public int Ap;
        public int ApDef;
        public int Coin;
        public int Critical; //暴击概率
        public int Crystal;
        public int Diamond;
        public int Dodge; //闪避概率

        public int Exp;

        public int GuideId;

        public int Hp;
        public int Id;
        public int Level;
        public string Name;
        public int Pierce; //穿透比率

        public int Power; //体力
        public int[] StrengthenArr;
        public string[] TaskArr;
        public long Time; //下线时间

        public int DungeonId;
    }

    #endregion

    #region 登陆相关

    [Serializable]
    public class RequestLogin
    {
        public string Account;
        public string Password;
    }

    [Serializable]
    public class ResponseLogin
    {
        public PlayerData PlayerData;
    }


    [Serializable]
    public class RequestRename
    {
        public string Name;
    }

    [Serializable]
    public class ResponseRename
    {
        public string Name;
    }

    #endregion

    #region 引导相关

    [Serializable]
    public class RequestGuide
    {
        public int GuideId;
    }

    [Serializable]
    public class ResponseGuide
    {
        public int Coin;
        public int Exp;
        public int GuideId;
        public int Level;
    }

    #endregion

    #region 强化相关

    [Serializable]
    public class RequestStrengthen
    {
        public int Position;
    }

    [Serializable]
    public class ResponseStrengthen
    {
        public int Ad;
        public int AdDef;
        public int Ap;
        public int ApDef;
        public int Coin;
        public int Crystal;
        public int Hp;
        public int[] StrengthenArr;
    }

    #endregion

    #region 聊天相关

    [Serializable]
    public class SendChat
    {
        public string Chat;
    }

    [Serializable]
    public class PushChat
    {
        public string Chat;
        public string Name;
    }

    #endregion

    #region 交易相关

    [Serializable]
    public class RequestBuy
    {
        public int Cost;
        public int Type;
    }

    [Serializable]
    public class ResponseBuy
    {
        public int Coin;
        public int Cost;
        public int Diamond;
        public int Power;
        public int Type;
    }

    #endregion

    #region 体力相关

    [Serializable]
    public class PushPower
    {
        public int Power;
    }

    #endregion

    #region 任务相关

    [Serializable]
    public class RequestTask
    {
        public int Id;
    }

    [Serializable]
    public class ResponseTask
    {
        public int Coin;
        public int Exp;
        public int Level;
        public string[] TaskArr;
    }

    [Serializable]
    public class PushTask
    {
        public string[] TaskArr;
    }

    #endregion

    #region 副本相关

    [Serializable]
    public class RequestDungeon
    {
        public int DungeonId;
    }

    [Serializable]
    public class ResponseDungeon
    {
        public int DungeonId;
        public int Power;
    }

    #endregion

    public enum ErrorCode
    {
        //账户未上线
        None,

        //账户已上线
        AccountIsOnline,

        //密码错误
        WrongPassword,

        //名字已经存在
        NameIsExist,

        //数据库更新出错
        UpDataDataBaseError,

        //服务器数据异常
        SeverDateError,

        //客户端数据异常
        ClientDataError,

        //数据异常
        LackLevel,
        LackCoin,
        LackCrystal,
        LackDiamond,
        LackPower,
    }

    public enum Cmd
    {
        //登陆相关
        RequestLogin = 101,
        ResponseLogin = 102,

        RequestRename = 103,
        ResponseRename = 104,

        //主城相关
        RequestGuide = 201,
        ResponseGuide = 202,

        RequestStrengthen = 203,
        ResponseStrengthen = 204,

        SendChat = 205,
        PushChat = 206,

        RequestBuy = 207,
        ResponseBuy = 208,

        PushPower = 209,

        RequestTask = 210,
        ResponseTask = 211,
        PushTask = 212,

        //副本相关
        RequestDungeon = 301,
        ResponseDungeon = 302,
    }

    public static class SeverConfig
    {
        public const string SeverIp = "192.168.31.237";
        public const int SeverPort = 17666;
    }
}