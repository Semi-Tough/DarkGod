/****************************************************
    文件：BuySystem
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月15日 星期二 17:36
    功能：交易系统
*****************************************************/

using PEProtocol;
using PETool.PELogger;

namespace Sever {
	public class BuySystem : SystemRoot<BuySystem> {
		public override void Init() {
			base.Init();
			PELogger.Log("BuySystem Init Done.");
		}

		public void ResponseBuy(MessagePack messagePack) {
			RequestBuy requestBuy = messagePack.message.Body.RequestBuy;
			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdResponseBuy
				},
				Body = new Body()
			};

			PlayerData? playerData = CacheSvc.Instance.GetPlayerDataBySession(messagePack.session);

			// PushTask pushTask = null;

			if(playerData != null && playerData.Diamond > requestBuy.Cost) {
				playerData.Diamond -= requestBuy.Cost;
				switch(requestBuy.Type) {
					case 0: {
						playerData.Coin += 1000;
						//检查任务进度
						TaskSystem.Instance.CheckTaskSingle(playerData, TaskType.BuyCoin);
						break;
					}
					case 1: {
						playerData.Power += 100;
						//检查任务进度
						TaskSystem.Instance.CheckTaskSingle(playerData, TaskType.BuyPower);
						break;
					}
				}


				if(cacheSvc.UpdateDatabase(playerData)) {
					message.Body.ResponseBuy = new ResponseBuy{
						Type = requestBuy.Type,
						Cost = requestBuy.Cost,
						Coin = playerData.Coin,
						Diamond = playerData.Diamond,
						Power = playerData.Power
					};
				}
				else {
					message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
				}
			}
			else {
				message.Head.ErrorCode = ErrorCode.ErrorCodeLackDiamond;
			}

			messagePack.session.SendMsg(Protocol.Serialize(message));
		}
	}
}