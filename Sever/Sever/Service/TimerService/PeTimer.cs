/****************************************************
	文件：PeTimer.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月21日 星期一 13:47   	
	功能：计时器
*****************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Sever
{
    public class PeTimer
    {
        private const string LockTime = "lock";
        private const string LockFrame = "lock";
        private const string LockTaskId = "lock";
        private readonly List<int> _deleteTasksList = new List<int>();

        private readonly List<PeFrameTask> _frameCacheList = new List<PeFrameTask>();
        private readonly List<PeFrameTask> _frameTasksList = new List<PeFrameTask>();
        private readonly List<int> _recycleIdList = new List<int>();

        private readonly Timer _severTimer;

        private readonly DateTime _startDateTime =
            new DateTime(1970, 1, 1, 0, 0, 0, 0);

        private readonly List<int> _tasksIdList = new List<int>();

        private readonly List<PeTimeTask> _timeCacheList = new List<PeTimeTask>();
        private readonly List<PeTimeTask> _timeTasksList = new List<PeTimeTask>();
        private int _currentFrame;

        private Action<Action, int> _taskHandle;
        private int _taskId;
        private Action<string> _taskLog;

        public PeTimer(int interval = 0)
        {
            _tasksIdList.Clear();
            _recycleIdList.Clear();
            _timeTasksList.Clear();
            _timeCacheList.Clear();
            _frameTasksList.Clear();
            _frameCacheList.Clear();

            if (interval == 0) return;
            //利用线程池技术,每隔X毫秒查找相对空闲的一个线程调用
            _severTimer = new Timer(interval);
            _severTimer.AutoReset = true;
            _severTimer.Elapsed += (sender, args) => { Update(); };
            _severTimer.Start();
        }

        public void Update()
        {
            _currentFrame += 1;
            CheckTimeTask();
            CheckFrameTask();
            DeleteTimeTasks();
            DeleteFrameTasks();

            if (_recycleIdList.Count <= 0) return;
            lock (LockTaskId)
            {
                RecycleTaskId();
            }
        }


        #region TimeTask

        public int AddTimeTask(Action callBack, double delay, int loopCount = 1,
            PeTimeUnit timeUnit = PeTimeUnit.Millisecond)
        {
            if (timeUnit != PeTimeUnit.Millisecond)
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

            int currentTaskId = GetTaskId();

            //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
            lock (LockTime)
            {
                _timeCacheList.Add(new PeTimeTask(currentTaskId, callBack, delay, GetUtcMillisecond() + delay,
                    loopCount));
            }

            return currentTaskId;
        }

        public void DeleteTimeTask(int taskId)
        {
            lock (LockTime)
            {
                _deleteTasksList.Add(taskId);
                LogInfo("DeleteList Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            }

            /*bool exist = false;
            for (int i = 0; i < _timeTasksList.TargetCount; i++)
            {
                PeTimeTask task = _timeTasksList[i];
                if (task.TaskId != taskId) continue;
                _timeTasksList.RemoveAt(i);
                _tasksIdList.Remove(taskId);
                exist = true;
                break;
            }

            if (exist) return true;

            for (int i = 0; i < _timeCacheList.TargetCount; i++)
            {
                PeTimeTask task = _timeCacheList[i];
                if (task.TaskId != taskId) continue;
                _timeCacheList.RemoveAt(i);
                _tasksIdList.Remove(taskId);
                exist = true;
                break;
            }

            return exist;*/
        }

        private void DeleteTimeTasks()
        {
            //将删除列表里的任务清除
            if (_deleteTasksList.Count <= 0) return;
            lock (LockTime)
            {
                foreach (int deleteId in _deleteTasksList)
                {
                    bool exist = false;
                    for (int i = 0; i < _timeTasksList.Count; i++)
                    {
                        PeTimeTask task = _timeTasksList[i];
                        if (task.TaskId != deleteId) continue;
                        _timeTasksList.RemoveAt(i);
                        _recycleIdList.Add(deleteId);
                        LogInfo("Delete Thread ID: " +
                                Thread.CurrentThread.ManagedThreadId);
                        exist = true;
                        break;
                    }

                    if (exist) continue;

                    for (int i = 0; i < _timeCacheList.Count; i++)
                    {
                        PeTimeTask task = _timeCacheList[i];
                        if (task.TaskId != deleteId) continue;
                        _timeCacheList.RemoveAt(i);
                        _recycleIdList.Add(deleteId);
                        LogInfo("Delete Thread ID: " +
                                Thread.CurrentThread.ManagedThreadId);
                        break;
                    }
                }

                _deleteTasksList.Clear();
            }
        }

        public bool ReplaceTimeTask(int taskId, Action callBack, double delay, int loopCount = 1,
            PeTimeUnit timeUnit = PeTimeUnit.Millisecond)
        {
            if (timeUnit != PeTimeUnit.Millisecond)
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

            //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
            PeTimeTask newTask = new PeTimeTask(taskId, callBack, delay, GetUtcMillisecond() + delay, loopCount);
            bool replaced = false;
            for (int i = 0; i < _timeTasksList.Count; i++)
            {
                if (_timeTasksList[i].TaskId != taskId) continue;
                _timeTasksList[i] = newTask;
                replaced = true;
                break;
            }

            if (replaced) return true;

            for (int i = 0; i < _timeCacheList.Count; i++)
            {
                if (_timeCacheList[i].TaskId != taskId) continue;
                _timeCacheList[i] = newTask;
                replaced = true;
                break;
            }

            return replaced;
        }

        private void CheckTimeTask()
        {
            //将缓存区中的任务取出来
            if (_timeCacheList.Count > 0)
            {
                lock (LockTime)
                {
                    foreach (PeTimeTask cacheTask in _timeCacheList) _timeTasksList.Add(cacheTask);
                }

                _timeCacheList.Clear();
            }

            //遍历检测任务是否达到条件
            if (_timeTasksList.Count <= 0) return;

            for (int i = 0; i < _timeTasksList.Count; i++)
            {
                PeTimeTask task = _timeTasksList[i];
                if (task.TargetTime > GetUtcMillisecond()) continue;

                Action callBack = task.CallBack;
                try
                {
                    if (_taskHandle != null)
                        _taskHandle(callBack, task.TaskId);
                    else
                        callBack?.Invoke();
                }
                catch (Exception e)
                {
                    LogInfo(e.ToString());
                }

                //移除已完成的任务,并且索引减一
                if (task.LoopCount == 1)
                {
                    _timeTasksList.RemoveAt(i);
                    i--;
                    _recycleIdList.Add(task.TaskId);
                }
                else
                {
                    if (task.LoopCount > 0) task.LoopCount -= 1;

                    task.TargetTime += task.DelayTime;
                }
            }
        }

        #endregion

        #region FrameTask

        public int AddFrameTask(Action callBack, int delay, int loopCount = 1)
        {
            int currentTaskId = GetTaskId();
            //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
            lock (LockFrame)
            {
                _frameTasksList.Add(new PeFrameTask(currentTaskId, callBack, delay,
                    _currentFrame + delay, loopCount));
            }

            _tasksIdList.Add(currentTaskId);
            return currentTaskId;
        }

        public void DeleteFrameTask(int taskId)
        {
            lock (LockFrame)
            {
                _deleteTasksList.Add(taskId);
                LogInfo("DeleteList Thread ID: " + Thread.CurrentThread.ManagedThreadId);
            }

            /*bool exist = false;
            for (int i = 0; i < _frameTasksList.TargetCount; i++)
            {
                PeFrameTask task = _frameTasksList[i];
                if (task.TaskId != taskId) continue;
                _frameTasksList.RemoveAt(i);
                _tasksIdList.Remove(taskId);
                exist = true;
                break;
            }

            if (exist) return true;

            for (int i = 0; i < _frameCacheList.TargetCount; i++)
            {
                PeFrameTask task = _frameCacheList[i];
                if (task.TaskId != taskId) continue;
                _frameCacheList.RemoveAt(i);
                _tasksIdList.Remove(taskId);
                exist = true;
                break;
            }

            return exist;*/
        }

        private void DeleteFrameTasks()
        {
            if (_deleteTasksList.Count <= 0) return;

            lock (LockFrame)
            {
                foreach (int deleteId in _deleteTasksList)
                {
                    bool exist = false;
                    for (int i = 0; i < _frameTasksList.Count; i++)
                    {
                        PeFrameTask task = _frameTasksList[i];
                        if (task.TaskId != deleteId) continue;
                        _frameTasksList.RemoveAt(i);
                        _recycleIdList.Add(deleteId);
                        LogInfo("Delete Thread ID: " +
                                Thread.CurrentThread.ManagedThreadId);

                        exist = true;
                        break;
                    }

                    if (exist) continue;

                    for (int i = 0; i < _frameCacheList.Count; i++)
                    {
                        PeFrameTask task = _frameCacheList[i];
                        if (task.TaskId != deleteId) continue;
                        _frameCacheList.RemoveAt(i);
                        _recycleIdList.Add(deleteId);
                        LogInfo("Delete Thread ID: " +
                                Thread.CurrentThread.ManagedThreadId);

                        break;
                    }
                }
            }
        }


        public bool ReplaceFrameTask(int taskId, Action callBack, int delay, int loopCount = 1)
        {
            //先将任务加入缓存,在下一帧开始任务,避免在for循环的时候加入任务
            PeFrameTask newTask = new PeFrameTask(taskId, callBack, delay,
                _currentFrame + delay, loopCount);

            bool replaced = false;
            for (int i = 0; i < _frameTasksList.Count; i++)
            {
                if (_frameTasksList[i].TaskId != taskId) continue;
                _frameTasksList[i] = newTask;
                replaced = true;
                break;
            }

            if (replaced) return true;

            for (int i = 0; i < _frameCacheList.Count; i++)
            {
                if (_frameCacheList[i].TaskId != taskId) continue;
                _frameCacheList[i] = newTask;
                replaced = true;
                break;
            }

            return replaced;
        }

        private void CheckFrameTask()
        {
            //将缓存区中的任务取出来
            if (_frameCacheList.Count > 0)
                lock (LockFrame)
                {
                    foreach (PeFrameTask cacheTask in _frameCacheList) _frameCacheList.Add(cacheTask);

                    _frameCacheList.Clear();
                }

            if (_frameTasksList.Count <= 0) return;
            //遍历检测任务是否达到条件
            for (int i = 0; i < _frameTasksList.Count; i++)
            {
                PeFrameTask task = _frameTasksList[i];
                if (task.TargetFrame > _currentFrame) continue;

                Action callBack = task.CallBack;
                try
                {
                    //在多线程中进行计时任务,计时完成后返回任务ID和任务回调，在主线程完成回调任务
                    if (_taskHandle != null)
                        _taskHandle(callBack, task.TaskId);
                    //在多线程中进行计时任务和回调
                    else
                        callBack?.Invoke();
                }
                catch (Exception e)
                {
                    LogInfo(e.ToString());
                }

                //移除已完成的任务,并且索引减一
                if (task.LoopCount == 1)
                {
                    _frameTasksList.RemoveAt(i);
                    i--;
                    _recycleIdList.Add(task.TaskId);
                }
                else
                {
                    if (task.LoopCount > 0) task.LoopCount -= 1;

                    task.TargetFrame += task.DelayFrame;
                }
            }
        }

        #endregion

        #region Tool Methods

        private int GetTaskId()
        {
            lock (LockTaskId)
            {
                _taskId += 1;

                while (true)
                {
                    //安全代码,以防万一
                    if (_taskId == int.MaxValue) _taskId = 0;


                    bool used = false;
                    foreach (int taskId in _tasksIdList)
                        if (_taskId == taskId)
                        {
                            used = true;
                            break;
                        }

                    if (used)
                        _taskId += 1;
                    else
                        break;
                }
            }

            _tasksIdList.Add(_taskId);
            return _taskId;
        }

        private void RecycleTaskId()
        {
            foreach (int recycleId in _recycleIdList) _tasksIdList.Remove(recycleId);

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

        public void SetHandle(Action<Action, int> handle)
        {
            _taskHandle = handle;
        }

        public double GetUtcMillisecond()
        {
            TimeSpan timeSpan = DateTime.UtcNow - _startDateTime;
            return timeSpan.TotalMilliseconds;
        }

        public int GetYear()
        {
            return GetLocalDateTime().Year;
        }

        public int GetMonth()
        {
            return GetLocalDateTime().Month;
        }

        public int GetDay()
        {
            return GetLocalDateTime().Day;
        }

        public int GetWeek()
        {
            return (int) GetLocalDateTime().DayOfWeek;
        }

        public string GetLocalTimeString()
        {
            DateTime dateTime = GetLocalDateTime();
            return GetTimeString(dateTime.Hour) + ":" + GetTimeString(dateTime.Minute) + ":" +
                   GetTimeString(dateTime.Second);
        }

        private DateTime GetLocalDateTime()
        {
            DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(_startDateTime.AddMilliseconds(_currentFrame));
            return dateTime;
        }

        private string GetTimeString(int time)
        {
            if (time < 10)
                return "0" + time;
            return time.ToString();
        }

        public void Reset()
        {
            _taskId = 0;
            _taskLog = null;
            _tasksIdList.Clear();
            _recycleIdList.Clear();

            _timeTasksList.Clear();
            _timeCacheList.Clear();

            _frameTasksList.Clear();
            _frameCacheList.Clear();

            _severTimer.Stop();
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
        public readonly Action CallBack;
        public readonly double DelayTime;
        public readonly int TaskId;
        public int LoopCount;
        public double TargetTime; //单位:毫秒

        public PeTimeTask(int taskId, Action callBack, double delayTime, double targetTime, int loopCount)
        {
            TaskId = taskId;
            CallBack = callBack;
            DelayTime = delayTime;
            TargetTime = targetTime;
            LoopCount = loopCount;
        }
    }

    public class PeFrameTask
    {
        public readonly Action CallBack;
        public readonly int DelayFrame;
        public readonly int TaskId;
        public int LoopCount;
        public int TargetFrame;

        public PeFrameTask(int taskId, Action callBack, int delayFrame, int targetFrame, int loopCount)
        {
            TaskId = taskId;
            CallBack = callBack;
            DelayFrame = delayFrame;
            TargetFrame = targetFrame;
            LoopCount = loopCount;
        }
    }
}