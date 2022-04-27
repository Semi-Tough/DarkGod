/****************************************************
	文件：PECommon.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/02/27 17:34   	
	功能：客户端和服务端共用工具类
*****************************************************/

using PENet;

namespace PEProtocol
{
    public enum PeLogType
    {
        Log = 0,
        Warn = 1,
        Error = 2,
        Info = 3
    }

    public static class PeCommon
    {
        public const int PowerAddInterval = 5; //分钟
        public const int PowerAddCount = 5; //分钟

        public static void Log(string message, PeLogType peLogType = PeLogType.Log)
        {
            LogLevel logLevel = (LogLevel) peLogType;
            PETool.LogMsg(message, logLevel);
        }

        public static int GetFightByProperty(PlayerData playerData)
        {
            return playerData.Level * 100 + playerData.Ad + playerData.Ap + playerData.AdDef +
                   playerData.ApDef;
        }

        public static int GetPowerLimit(int level)
        {
            return (level - 1) / 10 * 150 + 150;
        }

        public static int GetExpUpValueByLevel(int level)
        {
            return 100 * level * level;
        }

        public static void CalculateExp(PlayerData playerData, int addExp)
        {
            while (true)
            {
                int upgradeNeedExp = GetExpUpValueByLevel(playerData.Level) - playerData.Exp;
                if (addExp >= upgradeNeedExp)
                {
                    playerData.Level += 1;
                    playerData.Exp = 0;
                    addExp -= upgradeNeedExp;
                }
                else
                {
                    playerData.Exp += addExp;
                    break;
                }
            }
        }
    }
}