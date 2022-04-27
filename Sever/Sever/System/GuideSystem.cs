/****************************************************
    文件：GuideSystem
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月09日 星期三 20:14
    功能：引导系统
*****************************************************/

using PEProtocol;

namespace Sever
{
    public class GuideSystem : SystemRoot
    {
        private static GuideSystem _instance;
        public static GuideSystem Instance => _instance ?? (_instance = new GuideSystem());

        public override void Init()
        {
            base.Init();
            PeCommon.Log("GuideSystem Init Done.");
        }

        public void ResponseGuide(MessagePack messagePack)
        {
            RequestGuide requestGuide = messagePack.Message.RequestGuide;

            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.ResponseGuide
            };

            //更新引导ID
            PlayerData playerData = CacheService.GetPlayerDataBySession(messagePack.Session);
            GuideConfig guideConfig = ConfigService.GetGuideConfig(requestGuide.GuideId);

            if (playerData.GuideId == requestGuide.GuideId)
            {
                playerData.GuideId += 1;
                //更新玩家数据
                playerData.Coin += guideConfig.Coin;
                PeCommon.CalculateExp(playerData, guideConfig.Exp);

                //检查任务进度
                PushTask pushTask = null;
                if (playerData.GuideId >= 1001)
                {
                    pushTask = TaskSystem.Instance.GetPushTask(playerData, 1);
                }

                //更新数据库
                if (CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                {
                    message.ResponseGuide = new ResponseGuide
                    {
                        GuideId = playerData.GuideId,
                        Coin = playerData.Coin,
                        Level = playerData.Level,
                        Exp = playerData.Exp
                    };

                    //并包处理
                    message.PushTask = pushTask;
                }
                else
                {
                    message.err = (int) ErrorCode.UpDataDataBaseError;
                }
            }
            else
            {
                message.err = (int) ErrorCode.SeverDateError;
            }

            //发送给玩家
            messagePack.Session.SendMsg(message);
        }
    }
}