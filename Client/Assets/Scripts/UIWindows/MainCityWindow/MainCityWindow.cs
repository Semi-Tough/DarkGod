/****************************************************
    文件：MainCityWindow
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月02日 星期三 14:20
    功能：主城UI界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class MainCityWindow : WindowRoot
{
    public Image imgPowerPrg;
    public Image imgDirPoint;
    public Image imgDirBg;
    public Image imgTouch;

    public Text txtFight;
    public Text txtPower;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpProgress;

    public Button btnGuide;
    public GridLayoutGroup layoutGroup;
    public Animation menuAnimation;
    private GuideConfig _currentGuideDate;
    private MainCitySystem _mainCitySystem;

    private bool _menuState;
    private float _pointDistance;
    private Vector2 _startPosition;

    protected override void InitWindow()
    {
        base.InitWindow();
        _mainCitySystem = MainCitySystem.Instance;
        SetActive(imgDirPoint, false);
        RegisterTouchEvents();
        RefreshUI();
        _pointDistance = 1.0f * Screen.height / Constants.ScreenStandardHeight *
                         Constants.RockerOperateDistance;
    }

    public void RefreshUI()
    {
        SetText(txtName, playerData.Name);
        SetText(txtLevel, playerData.Level);
        SetText(txtFight, PeCommon.GetFightByProperty(playerData));

        SetText(txtPower,
            "体力: " + playerData.Power + "/" + PeCommon.GetPowerLimit(playerData.Level));
        imgPowerPrg.fillAmount = playerData.Power * 1.0f / PeCommon.GetPowerLimit(playerData.Level);

        #region 进度条UI

        int expPrgValue = (int) (playerData.Exp * 1.0f /
            PeCommon.GetExpUpValueByLevel(playerData.Level) * 100);

        txtExpProgress.text =
            playerData.Exp + "/" + PeCommon.GetExpUpValueByLevel(playerData.Level);

        int index = expPrgValue / 10;
        for (int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            Image image = layoutGroup.transform.GetChild(i).GetComponent<Image>();
            if (index > i)
                image.fillAmount = 1;
            else if (index == i)
                image.fillAmount = expPrgValue % 10 * 1.0f / 10;
            else
                image.fillAmount = 0;
        }

        float heightRatio = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float screenWeight = Screen.width * heightRatio;
        float cellSizeX = (screenWeight - 172) / 10;
        layoutGroup.cellSize = new Vector2(cellSizeX, 8);

        #endregion

        _currentGuideDate = resourceService.GetGuideConfig(playerData.GuideId);
        if (_currentGuideDate != null)
        {
            SetGuideBtnIcon(_currentGuideDate.npcId);
        }
    }


    private void RegisterTouchEvents()
    {
        OnClickDown(imgTouch.gameObject, eventDate =>
        {
            imgDirBg.transform.position = eventDate.position;
            _startPosition = eventDate.position;
            SetActive(imgDirPoint);
        });

        OnDrag(imgTouch.gameObject, eventDate =>
        {
            Vector2 dir = eventDate.position - _startPosition;

            if (dir.magnitude > _pointDistance)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, _pointDistance);
                imgDirPoint.transform.position = _startPosition + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = eventDate.position;
            }

            _mainCitySystem.SetPlayerBlend(dir.normalized);
        });

        OnClickUp(imgTouch.gameObject, eventData =>
        {
            imgDirBg.rectTransform.localPosition = Vector3.zero;
            imgDirPoint.rectTransform.localPosition = Vector3.zero;
            SetActive(imgDirPoint, false);

            _mainCitySystem.SetPlayerBlend(Vector2.zero);
        });
    }

    private void SetGuideBtnIcon(int npcId)
    {
        string spritePath = "";
        Image image = btnGuide.GetComponent<Image>();
        switch (npcId)
        {
            case Constants.NpcGuide:
                spritePath = PathDefine.GuideHead;
                break;
            case Constants.NpcWiseMan:
                spritePath = PathDefine.WiseHead;
                break;
            case Constants.NpcGeneralMan:
                spritePath = PathDefine.GeneralHead;
                break;
            case Constants.NpcArtisanMan:
                spritePath = PathDefine.ArtisanHead;
                break;
            case Constants.NpcTraderMan:
                spritePath = PathDefine.TraderHead;
                break;
        }

        SetSprite(image, spritePath);
    }

    #region 点击事件

    public void ClickMenuBtn()
    {
        audioService.PlayUiAudio(Constants.MenuClickBtn);
        _menuState = !_menuState;

        AnimationClip clip =
            menuAnimation.GetClip(_menuState ? "CloseMenuAnimation" : "OpenMenuAnimation");
        menuAnimation.Play(clip.name);

        // //利用动画倒播实现菜单开关
        // if (_menuState) {
        //     menuAnimation["CloseMenuAnimation"].time = 0f;
        //     menuAnimation["CloseMenuAnimation"].speed = 1.0f;
        // }
        // else {
        //     menuAnimation["CloseMenuAnimation"].time =
        //         menuAnimation["CloseMenuAnimation"].clip.length;
        //     menuAnimation["CloseMenuAnimation"].speed = -1.0f;
        // }
        // menuAnimation.Play("CloseMenuAnimation");
    }

    public void ClickInfoBtn()
    {
        audioService.PlayUiAudio(Constants.UiOpenPage);
        _mainCitySystem.OpenInfoWindow();
    }

    public void ClickGuideBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        if (_currentGuideDate != null)
            _mainCitySystem.RunTask(_currentGuideDate);
        else
            GameRoot.AddTips("更多任务正在开发中...");
    }

    public void ClickStrengthenBtn()
    {
        audioService.PlayUiAudio(Constants.UiOpenPage);
        _mainCitySystem.OpenStrengthenWindow();
    }

    public void ClickChatBtn()
    {
        audioService.PlayUiAudio(Constants.UiOpenPage);
        _mainCitySystem.OpenChatWindow();
    }

    public void ClickBuyVitBtn()
    {
        audioService.PlayUiAudio(Constants.UiOpenPage);
        _mainCitySystem.OpenBuyWindow(BuyType.Vit);
    }

    public void ClickBuyCoinBtn()
    {
        audioService.PlayUiAudio(Constants.UiOpenPage);
        _mainCitySystem.OpenBuyWindow(BuyType.Coin);
    }

    public void ClickTaskBtn()
    {
        audioService.PlayUiAudio(Constants.UiOpenPage);
        _mainCitySystem.OpenTaskWindow();
    }

    public void ClickDungeonBtn()
    {
        audioService.PlayUiAudio(Constants.UiOpenPage);
        _mainCitySystem.EnterDungeonWindow();
    }

    #endregion
}