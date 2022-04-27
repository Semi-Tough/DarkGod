/****************************************************
	文件：TaskWindow.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月24日 星期四 21:30   	
	功能：任务奖励界面
*****************************************************/

using System.Collections.Generic;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class TaskWindow : WindowRoot
{
    public ScrollRect scrollRect;
    public Transform content;
    private readonly List<TaskData> _taskDataList = new List<TaskData>();

    protected override void InitWindow()
    {
        base.InitWindow();
        timerService.AddFrameTask(() => { scrollRect.verticalNormalizedPosition = 1f; }, 1, 1);
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        _taskDataList.Clear();

        List<TaskData> unFinishedList = new List<TaskData>();
        List<TaskData> finishedList = new List<TaskData>();
        foreach (string taskArr in playerData.TaskArr)
        {
            string[] taskInfo = taskArr.Split('|');

            TaskData taskData = new TaskData
            {
                id = int.Parse(taskInfo[0]),
                progress = int.Parse(taskInfo[1]),
                finished = taskInfo[2].Equals("1")
            };

            if (taskData.finished)
            {
                finishedList.Add(taskData);
            }
            else
            {
                unFinishedList.Add(taskData);
            }
        }

        _taskDataList.AddRange(unFinishedList);
        _taskDataList.AddRange(finishedList);


        foreach (TaskData taskProgress in _taskDataList)
        {
            GameObject go = Instantiate(resourceService.LoadPrefab(PathDefine.TaskPrefab), content, true);
            go.transform.localScale = Vector3.one;
            TaskItem taskItem = go.GetComponent<TaskItem>();
            TaskConfig taskConfig = resourceService.GetTaskConfig(taskProgress.id);


            #region 迭代查找

            // SetText(GetTransform(go.transform, "TxtName"), taskConfig.taskName);
            //
            // SetText(GetTransform(go.transform, "TxtProgress"),
            //     taskData.progress + "/" + taskConfig.targetCount);
            //
            // SetText(GetTransform(go.transform, "TxtCoin"), "金币: " + taskConfig.coin);
            // SetText(GetTransform(go.transform, "TxtExp"), "经验: " + taskConfig.exp);
            //
            //
            // GetTransform(go.transform, "ImgFill").GetComponent<Image>().fillAmount =
            //     (float) taskData.progress / taskConfig.targetCount;
            //
            //
            // Transform rewardedImg = GetTransform(go.transform, "ImgReceived");
            // Button rewardBtn = GetTransform(go.transform, "BtnReward").GetComponent<Button>();

            #endregion

            SetText(taskItem.txtName, taskConfig.taskName);
            SetText(taskItem.txtProgress, taskProgress.progress + "/" + taskConfig.targetCount);
            SetText(taskItem.txtCoin, "金币: " + taskConfig.coin);
            SetText(taskItem.txtExp, "经验: " + taskConfig.exp);
            taskItem.imgFill.fillAmount = (float) taskProgress.progress / taskConfig.targetCount;
            Image rewardedImg = taskItem.imgFinished;
            Button rewardBtn = taskItem.btnReward;


            rewardBtn.onClick.AddListener(() =>
            {
                ClickRewardBtn(taskProgress);
                rewardedImg.gameObject.SetActive(false);
                rewardBtn.interactable = false;
            });

            if (taskProgress.finished)
            {
                rewardBtn.interactable = false;
                rewardedImg.gameObject.SetActive(true);
            }
            else
            {
                rewardedImg.gameObject.SetActive(false);
                rewardBtn.interactable = taskProgress.progress == taskConfig.targetCount;
            }
        }
    }


    #region 点击事件

    private void ClickRewardBtn(TaskData taskData)
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        GameMessage message = new GameMessage()
        {
            cmd = (int) Cmd.RequestTask,
            RequestTask = new RequestTask
            {
                Id = taskData.id
            }
        };
        netService.SendMessage(message);

        TaskConfig taskConfig = resourceService.GetTaskConfig(taskData.id);
        GameRoot.AddTips(Constants.Color(
            "任务奖励: 金币+" + taskConfig.coin + "   经验值+" + taskConfig.exp, TextColor.Blue));
    }

    public void ClickCloseBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        SetWindowState(false);
    }

    #endregion
}