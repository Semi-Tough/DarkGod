/****************************************************
    文件：StrengthenPanel.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/3/11 15:35:2
    功能：强化升级界面
*****************************************************/

using cfg;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
public class StrengthenPanel : PanelRoot {
	public Image ImgCurrentPosition;
	public Image ImgArrow1;
	public Image ImgArrow2;
	public Image ImgArrow3;
	public Text TxtStar;
	public Text TxtHp1;
	public Text TxtHurt1;
	public Text TxtDefense1;
	public Text TxtHp2;
	public Text TxtHurt2;
	public Text TxtDefense2;
	public Text TxtMinLevel;
	public Text TxtCostCoin;
	public Text TxtCostCrystal;
	public Text TxtCurrentCoin;

	public Transform BtnGroup;
	public Transform StarGroup;
	public Transform UpdateGroup;

	private readonly Image[] images = new Image[6];
	private int currentPosition;
	private int currentStarLevel;

	private StrengthenConfig strengthenConfig;

	override protected void InitPanel() {
		base.InitPanel();
		RegisterClickEvents();
		ClickBtnProperty(0);
	}

	private void RegisterClickEvents() {
		for(int i = 0; i < BtnGroup.childCount; i++) {
			Image image = BtnGroup.GetChild(i).GetComponent<Image>();
			OnClick(image.gameObject, obj => {
				audioSvc.Play2DAudio(Constants.UIClickBtn);
				ClickBtnProperty((int)obj);
			}, i);
			images[i] = image;
		}
	}

	private void ClickBtnProperty(int index) {
		currentPosition = index;
		currentStarLevel = playerData.StrengthenDatas[index].Level;

		for(int i = 0; i < images.Length; i++) {
			Transform trans = images[i].transform;
			if(i == index) {
				SetSprite(images[i], PathDefine.StrengthenSelectedBtnImg);
				trans.GetComponent<RectTransform>().sizeDelta = new Vector2(225, 95);
			}
			else {
				SetSprite(images[i], PathDefine.StrengthenCommonBtnImg);
				trans.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 80);
			}
		}

		RefreshDetailProperty();
	}

	private void RefreshDetailProperty() {
		SetText(TxtCurrentCoin, playerData.Coin);
		switch(currentPosition) {
			case(int)StrengthonType.Head:
				SetSprite(ImgCurrentPosition, PathDefine.HeadPosition);
				break;
			case(int)StrengthonType.Body:
				SetSprite(ImgCurrentPosition, PathDefine.BodyPosition);
				break;
			case(int)StrengthonType.Waist:
				SetSprite(ImgCurrentPosition, PathDefine.WaistPosition);
				break;
			case(int)StrengthonType.Hand:
				SetSprite(ImgCurrentPosition, PathDefine.HandPosition);
				break;
			case(int)StrengthonType.Leg:
				SetSprite(ImgCurrentPosition, PathDefine.LegPosition);
				break;
			case(int)StrengthonType.Feet:
				SetSprite(ImgCurrentPosition, PathDefine.FeetPosition);
				break;
		}

		SetText(TxtStar, currentStarLevel + "星级");
		for(int i = 0; i < StarGroup.childCount; i++) {
			Image image = StarGroup.GetChild(i).GetComponent<Image>();
			image.sprite =
				Resources.Load<Sprite>(currentStarLevel > i ? PathDefine.Star2 : PathDefine.Star1);
		}

		int addHp = GetCurrentPropertyValue(currentPosition, currentStarLevel, AddPropertyType.Hp);
		int adddef = GetCurrentPropertyValue(currentPosition, currentStarLevel, AddPropertyType.Defense);
		int addhurt = GetCurrentPropertyValue(currentPosition, currentStarLevel, AddPropertyType.Hurt);

		SetText(TxtHp1, $"生命值 + {addHp.ToString()} ");
		SetText(TxtDefense1, $"防御力 + {adddef.ToString()} ");
		SetText(TxtHurt1, $"伤害值 + {addhurt.ToString()}");


		int nextStarLevel = currentStarLevel + 1;
		strengthenConfig = resSvc.GetStrengthenConfig(currentPosition * 10 + nextStarLevel);

		if(strengthenConfig != null) {
			SetActive(TxtHp2);
			SetActive(TxtDefense2);
			SetActive(TxtHurt2);

			SetActive(ImgArrow1);
			SetActive(ImgArrow2);
			SetActive(ImgArrow3);

			SetActive(UpdateGroup);

			addHp += GetCurrentPropertyValue(currentPosition, nextStarLevel, AddPropertyType.Hp);
			adddef += GetCurrentPropertyValue(currentPosition, nextStarLevel, AddPropertyType.Defense);
			addhurt += GetCurrentPropertyValue(currentPosition, nextStarLevel, AddPropertyType.Hurt);

			SetText(TxtHp2, $"生命值 + {addHp.ToString()}");
			SetText(TxtDefense2, $"防御力 + {adddef.ToString()} ");
			SetText(TxtHurt2, $"伤害值 + {addhurt.ToString()} ");

			SetText(TxtMinLevel, strengthenConfig.Minlv);
			SetText(TxtCostCoin, strengthenConfig.Coin);
			SetText(TxtCostCrystal, strengthenConfig.Crystal + "/" + playerData.Crystal);
		}
		else {
			SetActive(TxtHp2, false);
			SetActive(TxtDefense2, false);
			SetActive(TxtHurt2, false);

			SetActive(ImgArrow1, false);
			SetActive(ImgArrow2, false);
			SetActive(ImgArrow3, false);

			SetActive(UpdateGroup, false);
		}
	}

	public void UpdateStrengthenProperty() {
		audioSvc.Play2DAudio(Constants.StrengthenClickBtn);
		ClickBtnProperty(currentPosition);
	}

	public enum AddPropertyType {
		Hp,
		Defense,
		Hurt
	}

	public int GetCurrentPropertyValue(int currentPos, int currentStar, AddPropertyType addPropertyType) {
		int result = 0;
		int strengthenId = 0;
		if(currentStarLevel > 0) {
			strengthenId = currentPos * 10 + currentStar;
		}

		StrengthenConfig config = resSvc.GetStrengthenConfig(strengthenId);
		if(config != null) {
			switch(addPropertyType) {
				case AddPropertyType.Hp:
					result += config.Addhp;
					break;
				case AddPropertyType.Defense:
					result += config.Adddef;
					break;
				case AddPropertyType.Hurt:
					result += config.Addhurt;
					break;
			}
		}

		return result;
	}
	#region 点击事件
	public void ClickCloseStrengthenBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		SetPanelState(false);
	}

	public void ClickStrengthenBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		if(currentStarLevel >= 10) {
			GameRoot.AddTips("该部位已强化到最高级...");
			return;
		}

		if(playerData.Level < strengthenConfig.Minlv) {
			GameRoot.AddTips("角色等级未达到强化要求...");
			return;
		}

		if(playerData.Coin < strengthenConfig.Coin) {
			GameRoot.AddTips("剩余金币不足...");
			return;
		}

		if(playerData.Crystal < strengthenConfig.Crystal) {
			GameRoot.AddTips("剩余水晶不足...");
			return;
		}

		netSvc.SendMessage(new NetMsg{
			Head = new Head(){
				Cmd = Cmd.CmdRequestStrengthen,
			},
			Body = new Body(){
				RequestStrengthen = new RequestStrengthen{
					StrengthenType = (StrengthenType)currentPosition
				}
			}
		});
	}
	#endregion
}