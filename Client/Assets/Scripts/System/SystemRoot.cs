/****************************************************
    文件：SystemRoot.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/23 14:18:59
    功能：业务系统基类
*****************************************************/

using PEProtocol;
using UnityEngine;

public class SystemRoot : MonoBehaviour
{
    protected AudioService audioService;
    protected ResourceService resourceService;
    protected NetService netService;
    protected TimerService timerService;
    protected GameRoot gameRoot;
    public virtual void InitSystem()
    {
        resourceService = ResourceService.instance;
        audioService = AudioService.instance;
        netService=NetService.instance;
        timerService=TimerService.instance;
        
        gameRoot=GameRoot.instance;
    }
}