/****************************************************
    文件：GuideWindow.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/3/9 15:19:23
    功能：引导对话界面
*****************************************************/

using PEProtocol;
using UnityEngine.UI;

public class GuideWindow : WindowRoot
{
    public Text txtTalk;
    public Text txtName;
    public Image imgIcon;

    private GuideConfig _currentGuideDate;
    private string[] _dialogArr;
    private int _index;

    protected override void InitWindow()
    {
        base.InitWindow();
        _currentGuideDate = MainCitySystem.Instance.GetGuideConfig();
        _dialogArr = _currentGuideDate.dialogArr.Split('#');
        _index = 1;
        SetTalk();
    }

    private void SetTalk()
    {
        string[] talkArr = _dialogArr[_index].Split('|');
        if (talkArr[0] == "0")
        {
            //玩家
            SetSprite(imgIcon, PathDefine.PlayerIcon);
            SetText(txtName,playerData.Name);
        }
        else
        {
            //NPC
            switch (_currentGuideDate.npcId)
            {
                case Constants.NpcWiseMan:
                    SetSprite(imgIcon, PathDefine.WiseIcon);
                    SetText(txtName, "智者");
                    break;
                case Constants.NpcGeneralMan:
                    SetSprite(imgIcon, PathDefine.GeneralIcon);
                    SetText(txtName, "将军");
                    break;
                case Constants.NpcArtisanMan:
                    SetSprite(imgIcon, PathDefine.ArtisanIcon);
                    SetText(txtName, "工匠");
                    break;
                case Constants.NpcTraderMan:
                    SetSprite(imgIcon, PathDefine.TraderIcon);
                    SetText(txtName, "商人");
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.GuideIcon);
                    SetText(txtName, "引导者");
                    break;
            }
        }

        SetText(txtTalk, talkArr[1].Contains("$name") ? talkArr[1].Replace("$name", playerData.Name) : talkArr[1]);

        imgIcon.SetNativeSize();
    }

    #region 点击事件

    public void ClickNextBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        _index += 1;
        if (_index == _dialogArr.Length)
        {
            //发送引导任务完成信息
            GameMessage msg = new GameMessage
            {
                cmd = (int) Cmd.RequestGuide,
                RequestGuide = new RequestGuide
                {
                    GuideId = _currentGuideDate.id
                }
            };

            netService.SendMessage(msg);
            SetWindowState(false);
        }
        else
        {
            SetTalk();
        }
    }

    #endregion
}