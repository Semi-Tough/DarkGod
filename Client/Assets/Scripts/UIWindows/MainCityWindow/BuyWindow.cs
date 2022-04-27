/****************************************************
    文件：BuyWindow
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月15日 星期二 15:55
    功能：交易购买界面
*****************************************************/

using PEProtocol;
using UnityEngine.UI;

public enum BuyType
{
    Coin,
    Vit
}

public class BuyWindow : WindowRoot
{
    public Text txtInfo;
    public Button btnBuy;
    private BuyType _currentType;


    protected override void InitWindow()
    {
        base.InitWindow();

        RefreshUI();
    }

    private void RefreshUI()
    {
        switch (_currentType)
        {
            case BuyType.Vit:
            {
                SetText(txtInfo,
                    "是否花费" + Constants.Color("10钻石", TextColor.Red) + "购买" +
                    Constants.Color("100体力", TextColor.Green));
            }
                break;
            case BuyType.Coin:
            {
                SetText(txtInfo,
                    "是否花费" + Constants.Color("10钻石", TextColor.Red) + "购买" +
                    Constants.Color("1000金币", TextColor.Yellow));
            }
                break;
        }
    }

    public void SetBuyType(BuyType type)
    {
        _currentType = type;
    }

    public void SetBtnInteractable()
    {
        btnBuy.interactable = true;
    }

    #region 点击事件

    public void ClickBuyBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);

        if (playerData.Diamond < 10)
        {
            GameRoot.AddTips("剩余钻石不足...");
            return;
        }

        //发送网络消息
        GameMessage message = new GameMessage
        {
            cmd = (int) Cmd.RequestBuy,
            RequestBuy = new RequestBuy
            {
                Type = (int) _currentType,
                Cost = 10
            }
        };
        btnBuy.interactable = false;
        netService.SendMessage(message);
    }

    public void ClickCloseBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        SetWindowState(false);
    }

    #endregion
}