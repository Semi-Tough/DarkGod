/****************************************************
	文件：DungeonSystem.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 09:26   	
	功能：副本系统
*****************************************************/

using PEProtocol;

namespace Sever
{
    public class DungeonSystem : SystemRoot
    {
        private static DungeonSystem _instance;

        public static DungeonSystem Instance => _instance ?? (_instance = new DungeonSystem());

        public override void Init()
        {
            base.Init();
            PeCommon.Log("DungeonSystem Init Done.");
        }

        public void ResponseDungeon(MessagePack messagePack)
        {
            RequestDungeon requestDungeon = messagePack.Message.RequestDungeon;

            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.ResponseDungeon,
            };
            PlayerData playerData = CacheService.GetPlayerDataBySession(messagePack.Session);
            int requestPower = ConfigService.GetMapConfig(requestDungeon.DungeonId).Power;

            if (playerData.DungeonId < requestDungeon.DungeonId)
            {
                message.err = (int) ErrorCode.ClientDataError;
            }
            else if (playerData.Power < requestPower)
            {
                message.err = (int) ErrorCode.LackPower;
            }
            else
            {
                playerData.Power -= requestPower;
                if (CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                {
                    ResponseDungeon responseDungeon = new ResponseDungeon
                    {
                        DungeonId = requestDungeon.DungeonId,
                        Power = playerData.Power
                    };

                    message.ResponseDungeon = responseDungeon;
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