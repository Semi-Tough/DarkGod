/****************************************************
	文件：PowerSystem.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月23日 星期三 19:50   	
	功能：体力系统
*****************************************************/

using PEProtocol;
using PETool.PELogger;

namespace Sever {
	public class PowerSystem : SystemRoot<PowerSystem> {
		public override void Init() {
			base.Init();

			TimerSvc.Instance.AddTimeTask(Protocol.PowerAddInterval, AddPower, null, -1);
			PELogger.Log("PowerSystem Init Done.");
		}

		private void AddPower(int tid) {
			//PELogger.Log("Add All Online Player Power");
			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdPushPower,
				},
				Body = new Body{
					PushPower = new PushPower()
				}
			};

			//计算所有在线玩家的体力增长
			Dictionary<SeverSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();

			foreach(KeyValuePair<SeverSession, PlayerData> onLinePlayer in onlineDic) {
				SeverSession session = onLinePlayer.Key;
				PlayerData playerData = onLinePlayer.Value;

				int maxPower = Protocol.GetPowerLimit(playerData.Level);

				if(playerData.Power >= maxPower) continue;

				playerData.LeaveTime = timerSvc.GetUtcMilliseconds();
				playerData.Power += Protocol.PowerAddCount;
				if(playerData.Power > maxPower) playerData.Power = maxPower;

				if(cacheSvc.UpdateDatabase(playerData)) {
					message.Body.PushPower.Power = playerData.Power;
					session.SendMsg(Protocol.Serialize(message));
				}
				else {
					message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
				}
			}
		}
	}
}