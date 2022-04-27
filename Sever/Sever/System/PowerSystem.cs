/****************************************************
	文件：PowerSystem.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月23日 星期三 19:50   	
	功能：体力系统
*****************************************************/

using System.Collections.Generic;
using PEProtocol;

namespace Sever
{
    public class PowerSystem : SystemRoot
    {
        private static PowerSystem _instance;
        public static PowerSystem Instance => _instance ?? (_instance = new PowerSystem());


        public override void Init()
        {
            base.Init();

            TimerService.Instance.AddTimeTask(AddPower, PeCommon.PowerAddInterval, 0,
                PeTimeUnit.Minute);
            PeCommon.Log("PowerSystem Init Done.");
        }

        private void AddPower()
        {
            //PeCommon.Log("Add All Online Player Power");
            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.PushPower,
                PushPower = new PushPower()
            };

            //计算所有在线玩家的体力增长
            Dictionary<SeverSession, PlayerData> onlineDic = CacheService.GetOnlineCache();

            foreach (KeyValuePair<SeverSession, PlayerData> onLinePlayer in onlineDic)
            {
                SeverSession session = onLinePlayer.Key;
                PlayerData playerData = onLinePlayer.Value;

                int maxPower = PeCommon.GetPowerLimit(playerData.Level);

                if (playerData.Power >= maxPower) continue;

                playerData.Time = TimerService.GetNowTime();
                playerData.Power += PeCommon.PowerAddCount;
                if (playerData.Power > maxPower) playerData.Power = maxPower;

                if (CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                {
                    message.PushPower.Power = playerData.Power;
                    session.SendMsg(message);
                }
                else
                {
                    message.err = (int) ErrorCode.UpDataDataBaseError;
                }
            }
        }
    }
}