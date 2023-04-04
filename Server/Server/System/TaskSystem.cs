/****************************************************
	文件：TaskSystem.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月25日 星期五 13:19   	
	功能：任务系统
*****************************************************/

using cfg;
using PEProtocol;
using PETool.PELogger;

namespace Sever {
	public class TaskSystem : SystemRoot<TaskSystem> {
		public override void Init() {
			base.Init();
			PELogger.Log("TaskSystem Init Done.");
		}

		public void ResponseTask(MessagePack messagePack) {
			RequestTask requestTask = messagePack.message.Body.RequestTask;
			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdResponseTask
				},
				Body = new Body()
			};
			PlayerData? playerData = CacheSvc.Instance.GetPlayerDataBySession(messagePack.session);
			TaskRewardConfig? taskConfig = ResSvc.Instance.GetTaskRewardConfig(requestTask.Id);
			if(playerData != null) {
				TaskData? taskData = playerData.TaskDatas[requestTask.Id];
				if(taskConfig != null && taskData.Progress == taskConfig.Count && !taskData.Finished) {
					playerData.Coin += taskConfig.Coin;
					Protocol.CalculateExp(playerData, taskConfig.Exp);
					taskData.Finished = true;
					playerData.TaskDatas[taskData.TaskId] = taskData;

					if(cacheSvc.UpdateDatabase(playerData)) {
						message.Body.ResponseTask = new ResponseTask{
							Coin = playerData.Coin,
							Exp = playerData.Exp,
							Level = playerData.Level,
							TaskLists = playerData.TaskDatas
						};
					}
					else {
						message.Head.ErrorCode = ErrorCode.ErrorCodeUpdataDatabaseError;
					}
				}
				else {
					message.Head.ErrorCode = ErrorCode.ErrorCodeClientDataError;
				}
			}
			messagePack.session.SendMsg(Protocol.Serialize(message));
		}


		public void CheckTaskSingle(PlayerData playerData, TaskType taskType) {
			TaskData taskData = playerData.TaskDatas[(int)taskType];
			TaskRewardConfig? taskConfig = resSvc.GetTaskRewardConfig((int)taskType);

			NetMsg message = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdPushTask
				},
				Body = new Body()
			};

			if(taskConfig == null) {
				message.Head.ErrorCode = ErrorCode.ErrorCodeServerDataError;
			}
			else {
				if(taskData.Progress >= taskConfig.Count || taskData.Finished) return;
			}

			taskData.Progress += 1;
			playerData.TaskDatas[(int)taskType] = taskData;
			SeverSession? session = cacheSvc.GetSessionByPlayerId(playerData.Id);

			message.Body.PushTask = new PushTask{ TaskLists = playerData.TaskDatas };

			// NetMsg message = new NetMsg{
			// 	Head = new Head{
			// 		Cmd = Cmd.CmdPushTask
			// 	},
			// 	Body = new Body{
			// 		PushTask = new PushTask{
			// 			TaskLists = playerData.TaskDatas
			// 		}
			// 	}
			// };

			session?.SendMsg(Protocol.Serialize(message));
		}
	}
	public enum TaskType {
		Conversion = 0,
		Battle = 1,
		Strengthen = 2,
		BuyPower = 3,
		BuyCoin = 4,
		Chat = 5,
	}
}