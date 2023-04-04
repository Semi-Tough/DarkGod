/****************************************************
	文件：TimerService.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月23日 星期三 14:37   	
	功能：计时服务
*****************************************************/

using System;
using UnityEngine;
using PETool.PELogger;
using PETool.PETimer;
public class TimerSvc : MonoBehaviour {
	public static TimerSvc Instance { private set; get; }
	private TickTimer tickTimer;
	public void InitService() {
		Instance = this;
		tickTimer = new TickTimer{
			logFunc = PELogger.Log,
			wainFunc = PELogger.Wain,
			errorFunc = PELogger.Error
		};
		PELogger.Log("计时服务加载完成");
	}

	private void Update() {
		tickTimer.UpdateTask();
		tickTimer.HandleTask();
	}

	public int AddTask(uint delay, Action<int> taskCB, int loopCount = 1, Action<int> cancelCB = null) {
		return tickTimer.AddTask(delay, taskCB, cancelCB, loopCount);
	}

	public bool RemoveTask(int tid) {
		return tickTimer.DeleteTask(tid);
	}
	public double GetNowTime() {
		return tickTimer.GetUtcMilliseconds();
	}
}