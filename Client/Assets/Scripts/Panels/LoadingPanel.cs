/****************************************************
    文件：LoadingPanel.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/22 14:31:19
    功能：加载进度界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
public class LoadingPanel : PanelRoot
{
    public Text TipsTxt;
    public Text LoadingProgress;
    public Image LoadingFg;
    public Image LoadingPoint;

    private float fgWidth;
    private float pointWidth;

    override protected void InitPanel()
    {
        base.InitPanel();
        fgWidth = LoadingFg.rectTransform.sizeDelta.x;
        pointWidth = LoadingPoint.rectTransform.sizeDelta.x;
        SetText(TipsTxt, "Tips:带有霸体状态的技能在施放时可以规避控制");
        SetText(LoadingProgress, "0%");
        LoadingFg.fillAmount = 0;
        LoadingPoint.rectTransform.anchoredPosition = new Vector2(-30, 0);
    }

    public void SetProgress(float progress)
    {
        SetText(LoadingProgress, (int) (progress * 100) + "%");
        LoadingFg.fillAmount = progress;
        float posX = progress * fgWidth - pointWidth / 2;
        LoadingPoint.rectTransform.anchoredPosition = new Vector2(posX, 0);
    }
}