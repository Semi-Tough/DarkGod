/****************************************************
    文件：StrengthenSystem
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月13日 星期日 16:21
    功能：强化系统
*****************************************************/

using PEProtocol;

namespace Sever
{
    public class StrengthenSystem : SystemRoot
    {
        private static StrengthenSystem _instance;
        
        public static StrengthenSystem Instance => _instance ?? (_instance = new StrengthenSystem());

        public override void Init()
        {
            base.Init();
            PeCommon.Log("StrengthenSystem Init Done.");
        }

        public void ResponseStrengthen(MessagePack messagePack)
        {
            RequestStrengthen requestStrengthen = messagePack.Message.RequestStrengthen;
            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.ResponseStrengthen
            };
            PlayerData playerData = CacheService.GetPlayerDataBySession(messagePack.Session);
            int currentStarLevel = playerData.StrengthenArr[requestStrengthen.Position];

            StrengthenConfig nextStrengthenConfig =
                ConfigService.GetStrengthenData(requestStrengthen.Position, currentStarLevel + 1);

            //判断是否满足升级条件
            if (playerData.Level < nextStrengthenConfig.MinLevel)
            {
                message.err = (int) ErrorCode.LackLevel;
            }
            else if (playerData.Coin < nextStrengthenConfig.Coin)
            {
                message.err = (int) ErrorCode.LackCoin;
            }
            else if (playerData.Crystal < nextStrengthenConfig.Crystal)
            {
                message.err = (int) ErrorCode.LackCrystal;
            }
            else
            {
                //扣除相应资源
                playerData.StrengthenArr[requestStrengthen.Position] += 1;
                playerData.Coin -= nextStrengthenConfig.Coin;
                playerData.Crystal -= nextStrengthenConfig.Crystal;
                //增加属性
                playerData.Hp += nextStrengthenConfig.AddHp;
                playerData.Ap += nextStrengthenConfig.AddHurt;
                playerData.Ad += nextStrengthenConfig.AddHurt;
                playerData.AdDef += nextStrengthenConfig.AddDefense;
                playerData.ApDef += nextStrengthenConfig.AddDefense;

                //检查任务进度
                PushTask pushTask = TaskSystem.Instance.GetPushTask(playerData, 3);

                //更新数据库
                if (CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                {
                    message.ResponseStrengthen = new ResponseStrengthen
                    {
                        Coin = playerData.Coin,
                        Crystal = playerData.Crystal,
                        Hp = playerData.Hp,
                        Ad = playerData.Ad,
                        Ap = playerData.Ap,
                        AdDef = playerData.AdDef,
                        ApDef = playerData.ApDef,
                        StrengthenArr = playerData.StrengthenArr
                    };
                    //并包处理
                    message.PushTask = pushTask;
                }
                else
                {
                    message.err = (int) ErrorCode.UpDataDataBaseError;
                }
            }

            messagePack.Session.SendMsg(message);
        }
    }
}