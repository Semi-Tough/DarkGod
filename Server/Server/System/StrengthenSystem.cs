/****************************************************
    文件：StrengthenSystem
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月13日 星期日 16:21
    功能：强化系统
*****************************************************/

using cfg;
using PEProtocol;
using PETool.PELogger;

namespace Sever {
	public class StrengthenSystem : SystemRoot<StrengthenSystem> {
		public override void Init() {
			base.Init();
			PELogger.Log("StrengthenSystem Init Done.");
		}

		public void ResponseStrengthen(MessagePack messagePack) {
			RequestStrengthen requestStrengthen = messagePack.message.Body.RequestStrengthen;
			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdResponseStrengthen
				},
				Body = new Body()
			};
			PlayerData? playerData = cacheSvc.GetPlayerDataBySession(messagePack.session);

			StrengthenData? strengthenData = playerData?.StrengthenDatas[(int)requestStrengthen.StrengthenType];

			if(strengthenData != null) {
				int currentStarLevel = strengthenData.Level;

				StrengthenConfig? nextStrengthenConfig =
					resSvc.GetStrengthenConfig((int)requestStrengthen.StrengthenType, currentStarLevel + 1);

				if(playerData == null || nextStrengthenConfig == null) return;

				//判断是否满足升级条件
				if(playerData.Level < nextStrengthenConfig.Minlv) {
					message.Head.ErrorCode = ErrorCode.ErrorCodeLackLevel;
				}
				else if(playerData.Coin < nextStrengthenConfig.Coin) {
					message.Head.ErrorCode = ErrorCode.ErrorCodeLackCoin;
				}
				else if(playerData.Crystal < nextStrengthenConfig.Crystal) {
					message.Head.ErrorCode = ErrorCode.ErrorCodeLackCrystal;
				}
				else {
					//扣除相应资源
					playerData.StrengthenDatas[(int)requestStrengthen.StrengthenType].Level += 1;
					playerData.Coin -= nextStrengthenConfig.Coin;
					playerData.Crystal -= nextStrengthenConfig.Crystal;
					//增加属性
					playerData.Hp += nextStrengthenConfig.Addhp;
					playerData.Ap += nextStrengthenConfig.Addhurt;
					playerData.Ad += nextStrengthenConfig.Addhurt;
					playerData.AdDef += nextStrengthenConfig.Adddef;
					playerData.ApDef += nextStrengthenConfig.Adddef;

					//检查任务进度
					// PushTask pushTask = TaskSystem.Instance.CheckTaskUnion(playerData, 3);
					TaskSystem.Instance.CheckTaskSingle(playerData, TaskType.Strengthen);


					//更新数据库
					if(cacheSvc.UpdateDatabase(playerData)) {
						message.Body.ResponseStrengthen = new ResponseStrengthen{
							Coin = playerData.Coin,
							Crystal = playerData.Crystal,
							Hp = playerData.Hp,
							Ad = playerData.Ad,
							Ap = playerData.Ap,
							AdDef = playerData.AdDef,
							ApDef = playerData.ApDef,
							StrengthenArrs = playerData.StrengthenDatas
						};
						//并包处理
						// message.Body.PushTask = pushTask;
					}
					else {
						message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
					}
				}
			}

			messagePack.session.SendMsg(Protocol.Serialize(message));
		}
	}
}