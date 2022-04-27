/****************************************************
    文件：StrengthenWindow.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/3/11 15:35:2
    功能：强化升级界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class StrengthenWindow : WindowRoot
{
    public Transform btnGroup;
    public Transform starGroup;
    public Transform updateGroup;

    public Image imgCurrentPosition;
    public Image imgPropertyArrow1;
    public Image imgPropertyArrow2;
    public Image imgPropertyArrow3;
    public Text txtStar;
    public Text txtPropertyHp1;
    public Text txtPropertyHurt1;
    public Text txtPropertyDefense1;
    public Text txtPropertyHp2;
    public Text txtPropertyHurt2;
    public Text txtPropertyDefense2;
    public Text txtMinLevel;
    public Text txtCostCoin;
    public Text txtCostCrystal;
    public Text txtCurrentCoin;

    private readonly Image[] _images = new Image[6];
    private int _currentPosition;
    private int _currentStarLevel;

    private StrengthenConfig _strengthenConfig;

    protected override void InitWindow()
    {
        base.InitWindow();
        RegisterClickEvents();
        ClickBtnProperty(0);
    }

    private void RegisterClickEvents()
    {
        for (int i = 0; i < btnGroup.childCount; i++)
        {
            Image image = btnGroup.GetChild(i).GetComponent<Image>();
            OnClick(image.gameObject, obj =>
            {
                audioService.PlayUiAudio(Constants.UiClickBtn);
                ClickBtnProperty((int) obj);
            }, i);
            _images[i] = image;
        }
    }

    private void ClickBtnProperty(int index)
    {
        _currentPosition = index;
        _currentStarLevel = playerData.StrengthenArr[_currentPosition];

        for (int i = 0; i < _images.Length; i++)
        {
            Transform trans = _images[i].transform;
            if (i == _currentPosition)
            {
                SetSprite(_images[i], PathDefine.StrengthenSelectedBtnImg);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(225, 95);
            }
            else
            {
                SetSprite(_images[i], PathDefine.StrengthenCommonBtnImg);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
            }
        }

        RefreshDetailProperty();
    }

    private void RefreshDetailProperty()
    {
        SetText(txtCurrentCoin, playerData.Coin);
        switch (_currentPosition)
        {
            case 0:
                SetSprite(imgCurrentPosition, PathDefine.HeadPosition);
                break;
            case 1:
                SetSprite(imgCurrentPosition, PathDefine.BodyPosition);
                break;
            case 2:
                SetSprite(imgCurrentPosition, PathDefine.WaistPosition);
                break;
            case 3:
                SetSprite(imgCurrentPosition, PathDefine.HandPosition);
                break;
            case 4:
                SetSprite(imgCurrentPosition, PathDefine.LegPosition);
                break;
            case 5:
                SetSprite(imgCurrentPosition, PathDefine.FeetPosition);
                break;
        }

        SetText(txtStar, _currentStarLevel + "星级");
        for (int i = 0; i < starGroup.childCount; i++)
        {
            Image image = starGroup.GetChild(i).GetComponent<Image>();
            image.sprite =
                Resources.Load<Sprite>(_currentStarLevel > i ? PathDefine.Star2 : PathDefine.Star1);
        }

        SetText(txtPropertyHp1,
            "生命值 + " + resourceService.GetCurrentPropertyValue(_currentPosition, _currentStarLevel,
                ResourceService.AddPropertyType.Hp));
        SetText(txtPropertyDefense1,
            "防御力 + " + resourceService.GetCurrentPropertyValue(_currentPosition, _currentStarLevel,
                ResourceService.AddPropertyType.Defense));
        SetText(txtPropertyHurt1,
            "伤害值 + " + resourceService.GetCurrentPropertyValue(_currentPosition, _currentStarLevel,
                ResourceService.AddPropertyType.Hurt));


        int nextStarLevel = _currentStarLevel + 1;
        _strengthenConfig = resourceService.GetStrengthenConfig(_currentPosition, nextStarLevel);

        if (_strengthenConfig != null)
        {
            SetActive(txtPropertyHp2);
            SetActive(txtPropertyDefense2);
            SetActive(txtPropertyHurt2);

            SetActive(imgPropertyArrow1);
            SetActive(imgPropertyArrow2);
            SetActive(imgPropertyArrow3);

            SetActive(updateGroup);


            SetText(txtPropertyHp2,
                "生命值 + " + resourceService.GetCurrentPropertyValue(_currentPosition, nextStarLevel,
                    ResourceService.AddPropertyType.Hp));
            SetText(txtPropertyDefense2,
                "防御力 + " + resourceService.GetCurrentPropertyValue(_currentPosition, nextStarLevel,
                    ResourceService.AddPropertyType.Defense));
            SetText(txtPropertyHurt2,
                "伤害值 + " + resourceService.GetCurrentPropertyValue(_currentPosition, nextStarLevel,
                    ResourceService.AddPropertyType.Hurt));

            SetText(txtMinLevel, _strengthenConfig.minLevel);
            SetText(txtCostCoin, _strengthenConfig.coin);
            SetText(txtCostCrystal, _strengthenConfig.crystal + "/" + playerData.Crystal);
        }
        else
        {
            SetActive(txtPropertyHp2, false);
            SetActive(txtPropertyDefense2, false);
            SetActive(txtPropertyHurt2, false);

            SetActive(imgPropertyArrow1, false);
            SetActive(imgPropertyArrow2, false);
            SetActive(imgPropertyArrow3, false);

            SetActive(updateGroup, false);
        }
    }

    public void UpdateStrengthenProperty()
    {
        audioService.PlayUiAudio(Constants.StrengthenClickBtn);
        ClickBtnProperty(_currentPosition);
    }

    #region 点击事件

    public void ClickCloseStrengthenBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        SetWindowState(false);
    }

    public void ClickStrengthenBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        if (_currentStarLevel >= 10)
        {
            GameRoot.AddTips("该部位已强化到最高级...");
            return;
        }

        if (playerData.Level < _strengthenConfig.minLevel)
        {
            GameRoot.AddTips("角色等级未达到强化要求...");
            return;
        }

        if (playerData.Coin < _strengthenConfig.coin)
        {
            GameRoot.AddTips("剩余金币不足...");
            return;
        }

        if (playerData.Crystal < _strengthenConfig.crystal)
        {
            GameRoot.AddTips("剩余水晶不足...");
            return;
        }

        netService.SendMessage(new GameMessage
        {
            cmd = (int) Cmd.RequestStrengthen,
            RequestStrengthen = new RequestStrengthen
            {
                Position = _currentPosition
            }
        });
    }

    #endregion
}