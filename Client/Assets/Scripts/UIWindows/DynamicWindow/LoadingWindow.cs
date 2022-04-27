/****************************************************
    文件：LoadingWindow.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/22 14:31:19
    功能：加载进度界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : WindowRoot
{
    public Text tipsTxt;
    public Text loadingProgress;
    public Image loadingFg;
    public Image loadingPoint;

    private float _fgWidth;
    private float _pointWidth;

    protected override void InitWindow()
    {
        base.InitWindow();

        _fgWidth = loadingFg.rectTransform.sizeDelta.x;
        _pointWidth = loadingPoint.rectTransform.sizeDelta.x;
        SetText(tipsTxt, "这是一条游戏提示...");
        SetText(loadingProgress, "0%");
        loadingFg.fillAmount = 0;
        loadingPoint.rectTransform.anchoredPosition = new Vector2(-30, 0);
    }

    public void SetProgress(float progress)
    {
        SetText(loadingProgress, (int) (progress * 100) + "%");
        loadingFg.fillAmount = progress;
        float posX = progress * _fgWidth - _pointWidth / 2;
        loadingPoint.rectTransform.anchoredPosition = new Vector2(posX, 0);
    }
}