/****************************************************
    文件：GuideSystem
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月09日 星期三 20:14
    功能：引导系统
*****************************************************/

using cfg;
using PEProtocol;
using PETool.PELogger;

namespace Sever {
	public class GuideSystem : SystemRoot<GuideSystem> {
		public override void Init() {
			base.Init();
			PELogger.Log("GuideSystem Init Done.");
		}

		public void ResponseGuide(MessagePack messagePack) {
			RequestGuide requestGuide = messagePack.message.Body.RequestGuide;

			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdResponseGuide
				},
				Body = new Body()
			};

			//更新引导ID
			PlayerData? playerData = cacheSvc.GetPlayerDataBySession(messagePack.session);

			GuideConfig? guideConfig = resSvc.GetGuideConfig(requestGuide.GuideId);

			if(playerData != null && playerData.GuideId == requestGuide.GuideId) {
				playerData.GuideId += 1;
				//更新玩家数据
				if(guideConfig != null) {
					playerData.Coin += guideConfig.Coin;
					Protocol.CalculateExp(playerData, guideConfig.Exp);
				}

				//检查任务进度
				// PushTask pushTask = null;
				// if(playerData.GuideId >= 1001) {
				// 	pushTask = TaskSystem.Instance.CheckTaskUnion(playerData, 1);
				// }

				TaskSystem.Instance.CheckTaskSingle(playerData, TaskType.Conversion);

				//更新数据库
				if(cacheSvc.UpdateDatabase(playerData)) {
					message.Body.ResponseGuide = new ResponseGuide{
						GuideId = playerData.GuideId,
						Coin = playerData.Coin,
						Level = playerData.Level,
						Exp = playerData.Exp
					};

					//并包处理
					// message.Body.PushTask = pushTask;
				}
				else {
					message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
				}
			}
			else {
				message.Head.ErrorCode = ErrorCode.ErrorCodeServerDataError;
			}

			//发送给玩家
			messagePack.session.SendMsg(Protocol.Serialize(message));
		}
	}
}