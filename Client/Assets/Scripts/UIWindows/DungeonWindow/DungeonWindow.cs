/****************************************************
	文件：DungeonWindow.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月27日 星期日 14:35   	
	功能：副本选择界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DungeonWindow : WindowRoot
{
    [FormerlySerializedAs("pointer")] public Transform pointerTransform;
    public Button[] btnDungeons;

    protected override void InitWindow()
    {
        base.InitWindow();
        RefreshUI();
    }

    private void RefreshUI()
    {
        int dungeonId = playerData.DungeonId;
        for (int i = 0; i < btnDungeons.Length; i++)
        {
            if (i < dungeonId % 10000)
            {
                SetActive(btnDungeons[i].gameObject);
                if (i != dungeonId % 10000 - 1) continue;
                pointerTransform.SetParent(btnDungeons[i].transform);
                pointerTransform.localPosition = new Vector3(60, 90, 0);
            }
            else
            {
                SetActive(btnDungeons[i].gameObject, false);
            }
        }
    }


    #region 点击事件

    public void ClickDungeonBtn(int dungeonId)
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);

        //检查玩家体力是否足够
        if (playerData.Power > resourceService.GetMapConfig(dungeonId).power)
        {
            //副本请求
            netService.SendMessage(new GameMessage
            {
                cmd = (int) Cmd.RequestDungeon,
                RequestDungeon = new RequestDungeon
                {
                    DungeonId = dungeonId
                }
            });
        }
        else
        {
            GameRoot.AddTips("剩余体力值不足..");
        }
    }

    public void ClickCloseBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        SetWindowState(false);
    }

    #endregion
}