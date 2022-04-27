/****************************************************
	文件：PeTimer.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月21日 星期一 13:47   	
	功能：计时器
*****************************************************/

using System;
using System.Collections.Generic;

public class PeTimer
{
    private readonly List<int> _tasksIdList = new List<int>();
    private readonly List<int> _recycleIdList = new List<int>();

    private readonly List<PeTimeTask> _timeTasksList = new List<PeTimeTask>();
    private readonly List<PeTimeTask> _timeCacheList = new List<PeTimeTask>();

    private readonly List<PeFrameTask> _frameTasksList = new List<PeFrameTask>();
    private readonly List<PeFrameTask> _frameCacheList = new List<PeFrameTask>();

    private readonly DateTime _startDateTime =
        new DateTime(1970, 1, 1, 0, 0, 0, 0);

    private Action<string> _taskLog;
    private const string Obj = "lock";
    private int _currentFrame;
    private int _taskId;

    public PeTimer()
    {
        _tasksIdList.Clear();
        _recycleIdList.Clear();
        _timeTasksList.Clear();
        _timeCacheList.Clear();
        _frameTasksList.Clear();
        _frameCacheList.Clear();
    }

    public void Update()
    {
        _currentFrame += 1;
        DetectionTimeTask();
        DetectionFrameTask();
        if (_recycleIdList.Count > 0)
        {
            RecycleTaskId();
        }
    }

    #region 时间任务

    public int AddTimeTask(Action callBack, double delay, int loopCount = 1,
        PeTimeUnit timeUnit = PeTimeUnit.Millisecond)
    {
        if (timeUnit != PeTimeUnit.Millisecond)
        {
            switch (timeUnit)
            {
                case PeTimeUnit.Second:
                    delay *= 1000;
                    break;
                case PeTimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case PeTimeUnit.Hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case PeTimeUnit.Day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    LogInfo("Add Task TimeUnit Type Error ...");
                    break;
            }
        }

        int currentTaskID = GetTaskID();

        //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
        _timeCacheList.Add(new PeTimeTask(currentTaskID, callBack, delay, GetUtcMillisecond() + delay, loopCount));
        _tasksIdList.Add(currentTaskID);
        return currentTaskID;
    }

    public bool DeleteTimeTask(int taskId)
    {
        bool exist = false;
        for (int i = 0; i < _timeTasksList.Count; i++)
        {
            PeTimeTask task = _timeTasksList[i];
            if (task.taskID != taskId) continue;
            _timeTasksList.RemoveAt(i);
            _tasksIdList.Remove(taskId);
            exist = true;
            break;
        }

        if (exist) return true;

        for (int i = 0; i < _timeCacheList.Count; i++)
        {
            PeTimeTask task = _timeCacheList[i];
            if (task.taskID != taskId) continue;
            _timeCacheList.RemoveAt(i);
            _tasksIdList.Remove(taskId);
            exist = true;
            break;
        }

        return exist;
    }

    public bool ReplaceTimeTask(int taskId, Action callBack, double delay, int loopCount = 1,
        PeTimeUnit timeUnit = PeTimeUnit.Millisecond)
    {
        if (timeUnit != PeTimeUnit.Millisecond)
        {
            switch (timeUnit)
            {
                case PeTimeUnit.Second:
                    delay *= 1000;
                    break;
                case PeTimeUnit.Minute:
                    delay = delay * 1000 * 60;
                    break;
                case PeTimeUnit.Hour:
                    delay = delay * 1000 * 60 * 60;
                    break;
                case PeTimeUnit.Day:
                    delay = delay * 1000 * 60 * 60 * 24;
                    break;
                default:
                    LogInfo("Add Task TimeUnit Type Error ...");
                    break;
            }
        }

        //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
        PeTimeTask newTask = new PeTimeTask(taskId, callBack, delay, GetUtcMillisecond() + delay, loopCount);
        bool replaced = false;
        for (int i = 0; i < _timeTasksList.Count; i++)
        {
            if (_timeTasksList[i].taskID != taskId) continue;
            _timeTasksList[i] = newTask;
            replaced = true;
            break;
        }

        if (replaced) return true;

        for (int i = 0; i < _timeCacheList.Count; i++)
        {
            if (_timeCacheList[i].taskID != taskId) continue;
            _timeCacheList[i] = newTask;
            replaced = true;
            break;
        }

        return replaced;
    }

    private void DetectionTimeTask()
    {
        //将缓存区中的任务取出来
        if (_timeCacheList.Count > 0)
        {
            foreach (PeTimeTask cacheTask in _timeCacheList)
            {
                _timeTasksList.Add(cacheTask);
            }

            _timeCacheList.Clear();
        }

        if (_timeTasksList.Count <= 0) return;

        //遍历检测任务是否达到条件
        for (int index = 0; index < _timeTasksList.Count; index++)
        {
            PeTimeTask task = _timeTasksList[index];
            if (task.targetTime > GetUtcMillisecond()) continue;

            Action action = task.callBack;
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                LogInfo(e.ToString());
            }

            //移除已完成的任务,并且索引减一
            if (task.loopCount == 1)
            {
                _timeTasksList.RemoveAt(index);
                index--;
                _recycleIdList.Add(task.taskID);
            }
            else
            {
                if (task.loopCount > 0)
                {
                    task.loopCount -= 1;
                }

                task.targetTime += task.delayTime;
            }
        }
    }

    #endregion

    #region 帧任务

    public int AddFrameTask(Action callBack, int delay, int loopCount = 1)
    {
        int currentTaskID = GetTaskID();
        //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
        _frameTasksList.Add(new PeFrameTask(currentTaskID, callBack, delay,
            _currentFrame + delay, loopCount));
        _tasksIdList.Add(currentTaskID);
        return currentTaskID;
    }

    public bool DeleteFrameTask(int taskId)
    {
        bool exist = false;
        for (int i = 0; i < _frameTasksList.Count; i++)
        {
            PeFrameTask task = _frameTasksList[i];
            if (task.taskID != taskId) continue;
            _frameTasksList.RemoveAt(i);
            _tasksIdList.Remove(taskId);
            exist = true;
            break;
        }

        if (exist) return true;

        for (int i = 0; i < _frameCacheList.Count; i++)
        {
            PeFrameTask task = _frameCacheList[i];
            if (task.taskID != taskId) continue;
            _frameCacheList.RemoveAt(i);
            _tasksIdList.Remove(taskId);
            exist = true;
            break;
        }

        return exist;
    }

    public bool ReplaceFrameTask(int taskId, Action callBack, int delay, int loopCount = 1)
    {
        //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
        PeFrameTask newTask = new PeFrameTask(taskId, callBack, delay,
            _currentFrame + delay, loopCount);

        bool replaced = false;
        for (int i = 0; i < _frameTasksList.Count; i++)
        {
            if (_frameTasksList[i].taskID != taskId) continue;
            _frameTasksList[i] = newTask;
            replaced = true;
            break;
        }

        if (replaced) return true;

        for (int i = 0; i < _frameCacheList.Count; i++)
        {
            if (_frameCacheList[i].taskID != taskId) continue;
            _frameCacheList[i] = newTask;
            replaced = true;
            break;
        }

        return replaced;
    }

    private void DetectionFrameTask()
    {
        //将缓存区中的任务取出来
        if (_frameCacheList.Count > 0)
        {
            foreach (PeFrameTask cacheTask in _frameCacheList)
            {
                _frameCacheList.Add(cacheTask);
            }

            _frameCacheList.Clear();
        }

        if (_frameTasksList.Count <= 0) return;
        //遍历检测任务是否达到条件
        for (int index = 0; index < _frameTasksList.Count; index++)
        {
            PeFrameTask task = _frameTasksList[index];
            if (task.targetFrame > _currentFrame) continue;

            Action action = task.callBack;
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                LogInfo(e.ToString());
            }

            //移除已完成的任务,并且索引减一
            if (task.loopCount == 1)
            {
                _frameTasksList.RemoveAt(index);
                index--;
                _recycleIdList.Add(task.taskID);
            }
            else
            {
                if (task.loopCount > 0)
                {
                    task.loopCount -= 1;
                }

                task.targetFrame += task.delayFrame;
            }
        }
    }

    #endregion

    #region 工具函数

    private int GetTaskID()
    {
        lock (Obj)
        {
            _taskId += 1;

            while (true)
            {
                //安全代码,以防万一
                if (_taskId == int.MaxValue)
                {
                    _taskId = 0;
                }

                bool used = false;

                foreach (int taskId in _tasksIdList)
                {
                    if (_taskId != taskId) continue;
                    used = true;
                    break;
                }

                if (used)
                {
                    _taskId += 1;
                }
                else
                {
                    break;
                }
            }
        }

        return _taskId;
    }

    private void RecycleTaskId()
    {
        foreach (int recycleId in _recycleIdList)
        {
            _tasksIdList.Remove(recycleId);
        }

        _recycleIdList.Clear();
    }

    public void SetLog(Action<string> log)
    {
        _taskLog = log;
    }

    private void LogInfo(string info)
    {
        _taskLog?.Invoke(info);
    }

    private double GetUtcMillisecond()
    {
        TimeSpan timeSpan = DateTime.UtcNow - _startDateTime;
        return timeSpan.TotalMilliseconds;
    }

    #endregion
}

public enum PeTimeUnit
{
    Millisecond,
    Second,
    Minute,
    Hour,
    Day
}

public class PeTimeTask
{
    public readonly int taskID;
    public readonly Action callBack;
    public readonly double delayTime;
    public double targetTime; //单位:毫秒
    public int loopCount;

    public PeTimeTask(int taskID, Action callBack, double delayTime, double targetTime, int loopCount)
    {
        this.taskID = taskID;
        this.callBack = callBack;
        this.delayTime = delayTime;
        this.targetTime = targetTime;
        this.loopCount = loopCount;
    }
}

public class PeFrameTask
{
    public readonly int taskID;
    public readonly Action callBack;
    public readonly int delayFrame;
    public int targetFrame;
    public int loopCount;

    public PeFrameTask(int taskID, Action callBack, int delayFrame, int targetFrame, int loopCount)
    {
        this.taskID = taskID;
        this.callBack = callBack;
        this.delayFrame = delayFrame;
        this.targetFrame = targetFrame;
        this.loopCount = loopCount;
    }
}