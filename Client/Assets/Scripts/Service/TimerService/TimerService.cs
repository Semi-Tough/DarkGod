/****************************************************
	文件：TimerService.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月23日 星期三 14:37   	
	功能：计时服务
*****************************************************/

using System;
using PEProtocol;
using UnityEngine;

public class TimerService : MonoBehaviour
{
    public static TimerService instance;

    private PeTimer _peTimer;

    public void InitService()
    {
        instance = this;
        _peTimer = new PeTimer();
        _peTimer.SetLog(info => { PeCommon.Log(info); });
        PeCommon.Log("计时服务加载完成");
    }

    private void Update()
    {
        _peTimer.Update();
    }

    public int AddTimeTask(Action callBack, double delay, int loopCount,
        PeTimeUnit peTimeUnit = PeTimeUnit.Millisecond)
    {
        return _peTimer.AddTimeTask(callBack, delay, loopCount, peTimeUnit);
    }

    public int AddFrameTask(Action callBack, int delay, int loopCount)
    {
        return _peTimer.AddFrameTask(callBack, delay, loopCount);
    }
}