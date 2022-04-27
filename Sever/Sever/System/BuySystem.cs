/****************************************************
    文件：BuySystem
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月15日 星期二 17:36
    功能：交易系统
*****************************************************/

using PEProtocol;

namespace Sever
{
    public class BuySystem : SystemRoot
    {
        private static BuySystem _instance;


        public static BuySystem Instance => _instance ?? (_instance = new BuySystem());

        public override void Init()
        {
            base.Init();
            PeCommon.Log("BuySystem Init Done.");
        }

        public void ResponseBuy(MessagePack messagePack)
        {
            RequestBuy requestBuy = messagePack.Message.RequestBuy;
            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.ResponseBuy
            };

            PlayerData playerData = CacheService.Instance.GetPlayerDataBySession(messagePack.Session);
            PushTask pushTask = null;

            if (playerData.Diamond > requestBuy.Cost)
            {
                playerData.Diamond -= requestBuy.Cost;
                switch (requestBuy.Type)
                {
                    case 0:
                    {
                        playerData.Coin += 1000;
                        //检查任务进度
                        pushTask = TaskSystem.Instance.GetPushTask(playerData, 5);
                        break;
                    }
                    case 1:
                    {
                        playerData.Power += 100;
                        //检查任务进度
                        pushTask = TaskSystem.Instance.GetPushTask(playerData, 4);
                        break;
                    }
                }


                if (CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                {
                    message.ResponseBuy = new ResponseBuy
                    {
                        Type = requestBuy.Type,
                        Cost = requestBuy.Cost,
                        Coin = playerData.Coin,
                        Diamond = playerData.Diamond,
                        Power = playerData.Power
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
                message.err = (int) ErrorCode.LackDiamond;
            }

            messagePack.Session.SendMsg(message);
        }
    }
}