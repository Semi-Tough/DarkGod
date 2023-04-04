/****************************************************
    文件：SystemRoot.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/23 14:18:59
    功能：业务系统基类
*****************************************************/

using UnityEngine;
public class SystemRoot : MonoBehaviour
{
    protected AudioSvc audioSvc;
    protected ResSvc resSvc;
    protected NetSvc netSvc;
    protected TimerSvc timerSvc;
    protected GameRoot gameRoot;
    public virtual void InitSystem()
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc=NetSvc.Instance;
        timerSvc=TimerSvc.Instance;
        
        gameRoot=GameRoot.Instance;
    }
}