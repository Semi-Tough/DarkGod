/****************************************************
	文件：TimerService.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月23日 星期三 18:43   	
	功能：定时服务
*****************************************************/

using System;
using System.Collections.Generic;
using PEProtocol;

namespace Sever
{
    public class TimerService
    {
        private const string TaskQueueLock = "lock";

        private static TimerService _instance;
        private readonly Queue<TaskPack> _taskPacksQueue = new Queue<TaskPack>();
        private PeTimer _peTimer;

        public static TimerService Instance => _instance ?? (_instance = new TimerService());

        public void Init()
        {
            _peTimer = new PeTimer(100);

            _peTimer.SetLog(info => { PeCommon.Log(info); });

            _peTimer.SetHandle((callBack, taskId) =>
            {
                if (callBack == null) return;
                lock (TaskQueueLock)
                {
                    _taskPacksQueue.Enqueue(new TaskPack(callBack));
                }
            });
            PeCommon.Log("TimerService Init Done.");
        }

        public void Update()
        {
            lock (TaskQueueLock)
            {
                while (_taskPacksQueue.Count > 0)
                {
                    TaskPack taskPack = _taskPacksQueue.Dequeue();
                    taskPack?.CallBack();
                }
            }
        }

        public int AddTimeTask(Action callBack, double delay, int loopCount = 1,
            PeTimeUnit timeUnit = PeTimeUnit.Millisecond)
        {
            return _peTimer.AddTimeTask(callBack, delay, loopCount, timeUnit);
        }

        public long GetNowTime()
        {
            return (long) _peTimer.GetUtcMillisecond();
        }

        private class TaskPack
        {
            public readonly Action CallBack;

            public TaskPack(Action callBack)
            {
                CallBack = callBack;
            }
        }
    }
}