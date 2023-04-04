/****************************************************
	文件：TaskPanel.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月24日 星期四 21:30   	
	功能：任务奖励界面
*****************************************************/

using cfg;
using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TaskPanel : PanelRoot {
	public Scrollbar Scrollbar;

	public Transform Content;
	private Dictionary<int, TaskItem> taskItemDic = new Dictionary<int, TaskItem>();

	private bool firstIn = true;
	private void FirstIn() {
		foreach(TaskRewardConfig config in resSvc.taskRewardDic.Values) {
			GameObject go = Instantiate(resSvc.LoadPrefab(PathDefine.TaskItem), Content, true);
			go.transform.localScale = Vector3.one;
			TaskItem taskItem = go.GetComponent<TaskItem>();
			TaskRewardConfig taskConfig = resSvc.GetTaskRewardConfig(config.Id);

			SetText(taskItem.TxtName, taskConfig.TaskName);
			SetText(taskItem.TxtProgress, 0 + "/" + taskConfig.Count);
			SetText(taskItem.TxtCoin, "金币: " + taskConfig.Coin);
			SetText(taskItem.TxtExp, "经验: " + taskConfig.Exp);
			taskItem.ImgFill.fillAmount = 0;
			Image rewardedImg = taskItem.ImgFinished;
			Button rewardBtn = taskItem.BtnReward;


			rewardBtn.onClick.AddListener(() => {
				ClickRewardBtn(config.Id);
				rewardedImg.gameObject.SetActive(false);
				rewardBtn.interactable = false;
			});
			taskItem.ImgFinished.gameObject.SetActive(false);
			taskItem.BtnReward.interactable = false;
			taskItemDic[config.Id] = taskItem;
		}
	}

	override protected void InitPanel() {
		base.InitPanel();
		if(firstIn) {
			FirstIn();
			firstIn = false;
		}
		RefreshTask();
	}


	public void RefreshTask() {
		foreach(TaskData taskData in playerData.TaskDatas) {
			TaskItem taskItem = taskItemDic[taskData.TaskId];
			TaskRewardConfig taskConfig = resSvc.GetTaskRewardConfig(taskData.TaskId);

			SetText(taskItem.TxtProgress, taskData.Progress + "/" + taskConfig.Count);
			SetText(taskItem.TxtCoin, "金币: " + taskConfig.Coin);
			SetText(taskItem.TxtExp, "经验: " + taskConfig.Exp);
			taskItem.ImgFill.fillAmount = (float)taskData.Progress / taskConfig.Count;

			if(taskData.Finished) {
				taskItem.BtnReward.interactable = false;
				taskItem.ImgFinished.gameObject.SetActive(true);
			}
			else {
				taskItem.ImgFinished.gameObject.SetActive(false);
				taskItem.BtnReward.interactable = taskData.Progress == taskConfig.Count;
			}
		}
	}

	#region 点击事件
	private void ClickRewardBtn(int taskId) {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		NetMsg message = new NetMsg{
			Head = new Head(){
				Cmd = Cmd.CmdRequestTask,
			},
			Body = new Body(){
				RequestTask = new RequestTask{
					Id = taskId
				}
			}
		};
		netSvc.SendMessage(message);
	}

	public void ClickCloseBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		SetPanelState(false);
	}
	#endregion
}