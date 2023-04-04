/****************************************************
	文件：LoginSystem.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：登陆系统
*****************************************************/

using PEProtocol;
using PETool.PELogger;
using Server;

namespace Sever {
	internal class LoginSystem : SystemRoot<LoginSystem> {
		public override void Init() {
			base.Init();
			PELogger.Log("LoginSystem Init Done.");
		}

		public void ResponseLogin(MessagePack messagePack) {
			RequestLogin data = messagePack.message.Body.RequestLogin;
			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdResponseLogin
				},
				Body = new Body()
			};

			//当前账户是否已经上线
			if(cacheSvc.IsAccountOnLine(data.Account)) {
				//已上线:返回错误消息
				message.Head.ErrorCode = ErrorCode.ErrorCodeAccountIsOnline;
			}
			else {
				//未上线:登陆账号
				//检测账户是否有存在
				PlayerData? playerData = cacheSvc.GetPlayerDataByAccount(data.Account);
				//创建默认的账户密码
				if(playerData == null) {
					playerData = new PlayerData{
						Name = "",
						Account = data.Account,
						Password = data.Password,

						Exp = 0,
						Level = 1,

						Coin = 5000,
						Diamond = 500,
						Crystal = 500,

						Hp = 2000,
						Ad = 275,
						Ap = 265,
						AdDef = 67,
						ApDef = 43,
						Dodge = 7,
						Pierce = 5,
						Critical = 2,

						GuideId = 1001,
						DungeonId = 10001,

						StrengthenDatas = new List<StrengthenData>(),
						TaskDatas = new List<TaskData>(),

						Power = 150,
						LeaveTime = timerSvc.GetUtcMilliseconds(),
					};

					for(int i = 0; i < 6; i++) {
						playerData.StrengthenDatas.Add(new StrengthenData{
							StrengthenType = (StrengthenType)i,
							Level = 0
						});
					}

					foreach(var task in resSvc.taskRewardDic.Values) {
						playerData.TaskDatas.Add(new TaskData{
							TaskId = task.Id,
							Progress = 0,
							MaxCount = task.Count,
							Finished = false,
						});
					}

					DbSvc.Instance.AddNewPlayerData(playerData);
				}


				//存在账户,密码错误
				if(playerData.Password != data.Password) {
					message.Head.ErrorCode = ErrorCode.ErrorCodeWrongPassword;
				}
				else {
					//存在账户,密码正确,拉取玩家数据
					//计算离线体力增长
					int power = playerData.Power;
					int maxPower = Protocol.GetPowerLimit(playerData.Level);

					if(power < maxPower) {
						double nowTime = timerSvc.GetUtcMilliseconds();
						double timeInterval = nowTime - playerData.LeaveTime;
						int addPower = (int)(timeInterval / (Protocol.PowerAddCount * 1000 * 60)) *
						               Protocol.PowerAddCount;

						if(addPower > 0) {
							playerData.Power += addPower;
							if(playerData.Power > maxPower) playerData.Power = maxPower;

							if(!cacheSvc.UpdateDatabase(playerData)) {
								PELogger.Error("Update Player Power Error");
							}
						}
					}

					message.Body.ResponseLogin = new ResponseLogin{
						PlayerData = playerData
					};
					cacheSvc.AccountOnline(data.Account, messagePack.session, playerData);
				}
			}

			//回应客户端
			messagePack.session.SendMsg(Protocol.Serialize(message));
		}

		public void ResponseRename(MessagePack messagePack) {
			RequestRename rename = messagePack.message.Body.RequestRename;
			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdResponseRename
				},
				Body = new Body()
			};

			if(!cacheSvc.IsNameExist(rename.Name)) {
				//更新缓存
				PlayerData? playerData = cacheSvc.GetPlayerDataBySession(messagePack.session);
				if(playerData != null) {
					playerData.Name = rename.Name;
					//数据库更新
					if(cacheSvc.UpdateDatabase(playerData))
						message.Body.ResponseRename = new ResponseRename{
							Name = rename.Name
						};
					else {
						message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
					}
				}
			}
			else {
				message.Head.ErrorCode = ErrorCode.ErrorCodeNameExist;
			}

			messagePack.session.SendMsg(Protocol.Serialize(message));
		}

		public void ClearOfflineData(SeverSession session) {
			//写入下线时间
			PlayerData? playerData = cacheSvc.GetPlayerDataBySession(session);
			if(playerData == null) return;
			playerData.LeaveTime = TimerSvc.Instance.GetUtcMilliseconds();
			if(!cacheSvc.UpdateDatabase(playerData))
				PELogger.Error("Update Offline Time Error");

			CacheSvc.Instance.AccountOffline(session);
		}
	}
}