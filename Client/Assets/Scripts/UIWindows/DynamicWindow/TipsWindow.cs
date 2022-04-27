/****************************************************
    文件：TipsWindow.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/23 15:17:54
    功能：动态UI元素界面
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsWindow : WindowRoot
{
    public Animation tipsAnimation;
    public Text tipsText;

    private readonly Queue<string> _tipsQueue = new Queue<string>();
    private string _lastTips;

    private void Update()
    {
        if (_tipsQueue.Count <= 0 || tipsText.gameObject.activeSelf) return;
        _lastTips = _tipsQueue.Dequeue();
        SetTips(_lastTips);
    }

    protected override void InitWindow()
    {
        SetActive(tipsText, false);
    }

    public void AddTips(string tips)
    {
        // if (_tipsQueue.Count > 0) {
        //     if (_tipsQueue.ToArray()[_tipsQueue.Count - 1] == tips) {
        //         return;
        //     }
        // }
        if (tips == _lastTips && _tipsQueue.Count != 0) return;
        _tipsQueue.Enqueue(tips);
    }

    private void SetTips(string tips)
    {
        SetActive(tipsText);
        SetText(tipsText, tips);

        tipsAnimation["TipsAnimation"].speed = _tipsQueue.Count + 1;
        tipsAnimation.Play();
    }
    
    
}