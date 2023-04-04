/****************************************************
	文件：DungeonSystem.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 09:26   	
	功能：副本系统
*****************************************************/

using cfg;
using PEProtocol;
using PETool.PELogger;

namespace Sever {
	public class DungeonSystem : SystemRoot<DungeonSystem> {
		public override void Init() {
			base.Init();
			PELogger.Log("DungeonSystem Init Done.");
		}

		public void ResponseDungeon(MessagePack messagePack) {
			RequestDungeon requestDungeon = messagePack.message.Body.RequestDungeon;

			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdResponseDungeon
				},
				Body = new Body()
			};
			PlayerData? playerData = cacheSvc.GetPlayerDataBySession(messagePack.session);
			int requestPower = resSvc.GetMapConfig(requestDungeon.DungeonId)!.Power;

			if(playerData != null && playerData.DungeonId < requestDungeon.DungeonId) {
				message.Head.ErrorCode = ErrorCode.ErrorCodeClientDataError;
			}
			else if(playerData != null && playerData.Power < requestPower) {
				message.Head.ErrorCode = ErrorCode.ErrorCodeLackPower;
			}
			else {
				if(playerData != null) {
					playerData.Power -= requestPower;
					if(cacheSvc.UpdateDatabase(playerData)) {
						ResponseDungeon responseDungeon = new ResponseDungeon{
							DungeonId = requestDungeon.DungeonId,
							Power = playerData.Power
						};

						message.Body.ResponseDungeon = responseDungeon;
					}
					else {
						message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
					}
				}
			}

			messagePack.session.SendMsg(Protocol.Serialize(message));
		}
		public void ResponseEndDungeon(MessagePack messagePack) {
			RequestEndDungeon data = messagePack.message.Body.RequestEndDungeon;
			NetMsg message = new NetMsg(){
				Head = new Head{
					Cmd = Cmd.CmdResponseEndDungeon
				},
				Body = new Body()
			};
			if(data.Win) {
				if(data is{ RestHp: > 0, CostTime: > 0 }) {
					MapConfig? map = resSvc.GetMapConfig(data.DungeonId);
					PlayerData? playerData = cacheSvc.GetPlayerDataBySession(messagePack.session);
					if(playerData != null) {
						TaskSystem.Instance.CheckTaskSingle(playerData, TaskType.Battle);
						if(map != null) {
							playerData.Coin += map.Coin;
							playerData.Crystal += map.Crystal;
							Protocol.CalculateExp(playerData, map.Exp);
						}
						if(playerData.DungeonId == data.DungeonId) {
							playerData.DungeonId += 1;
						}
						if(!cacheSvc.UpdateDatabase(playerData)) {
							message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
						}
						else {
							ResponseEndDungeon responseEndDungeon = new ResponseEndDungeon{
								Win = data.Win,
								DungeonId = data.DungeonId,
								RestHp = data.RestHp,
								CostTime = data.CostTime,

								Coin = playerData.Coin,
								Level = playerData.Level,
								Exp = playerData.Exp,
								Crystal = playerData.Crystal,
								Dungeon = playerData.DungeonId
							};
							message.Body.ResponseEndDungeon = responseEndDungeon;
						}
					}
				}
			}
			else {
				message.Head.ErrorCode = ErrorCode.ErrorCodeClientDataError;
			}
			messagePack.session.SendMsg(Protocol.Serialize(message));
		}
	}
}