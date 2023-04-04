/****************************************************
    文件：TipsPanel.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/23 15:17:54
    功能：动态UI元素界面
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TipsPanel : PanelRoot
{
    public Text TxtTips;
    
    public Animation TipsAnimation;
    private readonly Queue<string> tipsQueue = new Queue<string>();
    private string lastTips;

    private void Update()
    {
        if (tipsQueue.Count <= 0 || TxtTips.gameObject.activeSelf) return;
        SetTips(tipsQueue.Dequeue());
    }

    override protected void InitPanel()
    {
        SetActive(TxtTips, false);
    }

    public void AddTips(string tips)
    {
        if (tips == lastTips && tipsQueue.Count != 0) return;
        tipsQueue.Enqueue(tips);
        lastTips = tips;
    }

    private void SetTips(string tips)
    {
        SetActive(TxtTips);
        SetText(TxtTips, tips);

        TipsAnimation["TipsAnimation"].speed = tipsQueue.Count + 1;
        TipsAnimation.Play();
    }
}