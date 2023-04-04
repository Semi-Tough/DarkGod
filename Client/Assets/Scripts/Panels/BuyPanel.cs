/****************************************************
    文件：BuyPanel
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月15日 星期二 15:55
    功能：交易购买界面
*****************************************************/

using PEProtocol;
using UnityEngine.UI;
public enum BuyType {
	Coin,
	Vit
}

public class BuyPanel : PanelRoot {
	public Text TxtInfo;
	public Button BtnBuy;
	public BuyType CurrentType;


	override protected void InitPanel() {
		base.InitPanel();
		RefreshUI();
	}

	private void RefreshUI() {
		switch(CurrentType) {
			case BuyType.Vit: {
				SetText(TxtInfo,
					"是否花费" +
					Constants.Color("10钻石", TextColor.Red) +
					"购买" +
					Constants.Color("100体力", TextColor.Green));
			}
				break;
			case BuyType.Coin: {
				SetText(TxtInfo,
					"是否花费" +
					Constants.Color("10钻石", TextColor.Red) +
					"购买" +
					Constants.Color("1000金币", TextColor.Yellow));
			}
				break;
		}
	}

	public void SetBuyType(BuyType type) {
		CurrentType = type;
	}

	public void SetBtnInteractable() {
		BtnBuy.interactable = true;
	}

	#region 点击事件
	public void ClickBuyBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);

		if(playerData.Diamond < 10) {
			GameRoot.AddTips("剩余钻石不足...");
			return;
		}

		//发送网络消息
		NetMsg message = new NetMsg{
			Head = new Head{
				Cmd = Cmd.CmdRequestBuy,
			},
			Body = new Body{
				RequestBuy = new RequestBuy{
					Type = (int)CurrentType,
					Cost = 10
				}
			}
		};
		BtnBuy.interactable = false;
		netSvc.SendMessage(message);
	}

	public void ClickCloseBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		SetPanelState(false);
	}
	#endregion
}