/****************************************************
	文件：TaskSystem.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月25日 星期五 13:19   	
	功能：任务系统
*****************************************************/

using PEProtocol;

namespace Sever
{
    public class TaskSystem : SystemRoot
    {
        private static TaskSystem _instance;
        public static TaskSystem Instance => _instance ?? (_instance = new TaskSystem());

        public override void Init()
        {
            base.Init();
            PeCommon.Log("TaskSystem Init Done.");
        }

        public void ResponseTask(MessagePack messagePack)
        {
            RequestTask requestTask = messagePack.Message.RequestTask;

            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.ResponseTask
            };

            PlayerData playerData = CacheService.GetPlayerDataBySession(messagePack.Session);
            TaskConfig taskConfig = ConfigService.GetTaskConfig(requestTask.Id);
            TaskProgress taskProgress = GetTaskProgress(playerData, requestTask.Id);
            if (taskProgress.Progress == taskConfig.TargetCount && !taskProgress.Finished)
            {
                playerData.Coin += taskConfig.Coin;
                PeCommon.CalculateExp(playerData, taskConfig.Exp);
                taskProgress.Finished = true;
                SetTaskArr(playerData, taskProgress);

                if (CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                    message.ResponseTask = new ResponseTask
                    {
                        Coin = playerData.Coin,
                        Exp = playerData.Exp,
                        Level = playerData.Level,
                        TaskArr = playerData.TaskArr
                    };
                else
                {
                    message.err = (int) ErrorCode.UpDataDataBaseError;
                }
            }
            else
            {
                message.err = (int) ErrorCode.ClientDataError;
            }


            messagePack.Session.SendMsg(message);
        }

        private static TaskProgress GetTaskProgress(PlayerData playerData, int requestId)
        {
            TaskProgress newProgress = null;
            foreach (string taskProgress in playerData.TaskArr)
            {
                string[] taskInfo = taskProgress.Split('|');
                if (int.Parse(taskInfo[0]) != requestId) continue;
                newProgress = new TaskProgress
                {
                    Id = int.Parse(taskInfo[0]),
                    Progress = int.Parse(taskInfo[1]),
                    Finished = taskInfo[2].Equals("1")
                };
                break;
            }

            return newProgress;
        }

        private static void SetTaskArr(PlayerData playerData, TaskProgress taskProgress)
        {
            string newTask = taskProgress.Id + "|" + taskProgress.Progress + "|" +
                             (taskProgress.Finished ? 1 : 0);
            int index = -1;
            for (int i = 0; i < playerData.TaskArr.Length; i++)
            {
                string[] taskInfo = playerData.TaskArr[i].Split('|');
                if (int.Parse(taskInfo[0]) != taskProgress.Id) continue;
                index = i;
                break;
            }

            playerData.TaskArr[index] = newTask;
        }

        public void CheckTaskProgress(PlayerData playerData, int taskId)
        {
            TaskProgress taskProgress = GetTaskProgress(playerData, taskId);
            TaskConfig taskConfig = ConfigService.GetTaskConfig(taskId);

            if (taskProgress.Progress >= taskConfig.TargetCount) return;
            taskProgress.Progress += 1;
            SetTaskArr(playerData, taskProgress);
            SeverSession session = CacheService.GetSessionByPlayerId(playerData.Id);

            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.PushTask,
                PushTask = new PushTask
                {
                    TaskArr = playerData.TaskArr
                }
            };
            session.SendMsg(message);
        }

        public PushTask GetPushTask(PlayerData playerData, int taskId)
        {
            TaskProgress taskProgress = GetTaskProgress(playerData, taskId);
            TaskConfig taskConfig = ConfigService.GetTaskConfig(taskId);

            if (taskProgress.Progress >= taskConfig.TargetCount) return null;

            taskProgress.Progress += 1;
            SetTaskArr(playerData, taskProgress);
            return new PushTask
            {
                TaskArr = playerData.TaskArr
            };
        }
    }
}