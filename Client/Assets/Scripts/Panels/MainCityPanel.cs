/****************************************************
    文件：MainCityPanel
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月02日 星期三 14:20
    功能：主城UI界面
*****************************************************/

using cfg;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
public class MainCityPanel : PanelRoot {
	public Image ImgPowerPrg;
	public Image ImgDirPoint;
	public Image ImgDirBg;
	public Image ImgTouch;
	public Text TxtFight;
	public Text TxtPower;
	public Text TxtLevel;
	public Text TxtName;
	public Text TxtExpProgress;
	public Button BtnGuide;
	public GridLayoutGroup ExpImgGroup;


	public Animation MenuAnimation;
	private GuideConfig currentGuideDate;
	private MainCitySys mainCitySys;
	private bool menuState;
	private float pointDistance;
	private Vector2 startPosition;

	override protected void InitPanel() {
		base.InitPanel();
		mainCitySys = MainCitySys.Instance;
		SetActive(ImgDirPoint, false);
		RegisterTouchEvents();
		RefreshUI();
		pointDistance = 1.0f *
		                Screen.height /
		                Constants.ScreenStandardHeight *
		                Constants.RockerOperateDistance;
	}

	public void RefreshUI() {
		SetText(TxtName, playerData.Name);
		SetText(TxtLevel, playerData.Level);
		SetText(TxtFight, Protocol.GetFightByProperty(playerData));

		SetText(TxtPower,
			"体力: " + playerData.Power + "/" + Protocol.GetPowerLimit(playerData.Level));
		ImgPowerPrg.fillAmount = playerData.Power * 1.0f / Protocol.GetPowerLimit(playerData.Level);

		#region 进度条UI
		int expPrgValue = (int)(playerData.Exp *
		                        1.0f /
		                        Protocol.GetExpUpValueByLevel(playerData.Level) *
		                        100);

		TxtExpProgress.text =
			playerData.Exp + "/" + Protocol.GetExpUpValueByLevel(playerData.Level);

		int index = expPrgValue / 10;
		for(int i = 0; i < ExpImgGroup.transform.childCount; i++) {
			Image image = ExpImgGroup.transform.GetChild(i).GetComponent<Image>();
			if(index > i)
				image.fillAmount = 1;
			else if(index == i)
				image.fillAmount = expPrgValue % 10 * 1.0f / 10;
			else
				image.fillAmount = 0;
		}

		float heightRatio = 1.0f * Constants.ScreenStandardHeight / Screen.height;
		float screenWeight = Screen.width * heightRatio;
		float cellSizeX = (screenWeight - 172) / 10;
		ExpImgGroup.cellSize = new Vector2(cellSizeX, 8);
		#endregion

		currentGuideDate = resSvc.GetGuideConfig(playerData.GuideId);
		if(currentGuideDate != null) {
			SetGuideBtnIcon(currentGuideDate.NpcId);
		}
	}

	private void RegisterTouchEvents() {
		OnClickDown(ImgTouch.gameObject, eventDate => {
			ImgDirBg.transform.position = eventDate.position;
			startPosition = eventDate.position;
			SetActive(ImgDirPoint);
		});

		OnDrag(ImgTouch.gameObject, eventDate => {
			Vector2 dir = eventDate.position - startPosition;

			if(dir.magnitude > pointDistance) {
				Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDistance);
				ImgDirPoint.transform.position = startPosition + clampDir;
			}
			else {
				ImgDirPoint.transform.position = eventDate.position;
			}

			mainCitySys.SetPlayerBlend(dir.normalized);
		});

		OnClickUp(ImgTouch.gameObject, eventData => {
			ImgDirBg.rectTransform.localPosition = Vector3.zero;
			ImgDirPoint.rectTransform.localPosition = Vector3.zero;
			SetActive(ImgDirPoint, false);

			mainCitySys.SetPlayerBlend(Vector2.zero);
		});
	}

	private void SetGuideBtnIcon(int npcId) {
		string spritePath = "";
		Image image = BtnGuide.GetComponent<Image>();
		switch(npcId) {
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
	public void ClickMenuBtn() {
		audioSvc.Play2DAudio(Constants.MenuClickBtn);
		menuState = !menuState;

		AnimationClip clip =
			MenuAnimation.GetClip(menuState ? "CloseMenuAnimation" : "OpenMenuAnimation");
		MenuAnimation.Play(clip.name);

	}

	public void ClickInfoBtn() {
		audioSvc.Play2DAudio(Constants.UIOpenPage);
		mainCitySys.OpenInfoPanel();
	}

	public void ClickGuideBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		if(currentGuideDate != null)
			mainCitySys.RunTask(currentGuideDate);
		else
			GameRoot.AddTips("更多任务正在开发中...");
	}

	public void ClickStrengthenBtn() {
		audioSvc.Play2DAudio(Constants.UIOpenPage);
		mainCitySys.OpenStrengthenPanel();
	}

	public void ClickChatBtn() {
		audioSvc.Play2DAudio(Constants.UIOpenPage);
		mainCitySys.OpenChatPanel();
	}

	public void ClickBuyVitBtn() {
		audioSvc.Play2DAudio(Constants.UIOpenPage);
		mainCitySys.OpenBuyPanel(BuyType.Vit);
	}

	public void ClickBuyCoinBtn() {
		audioSvc.Play2DAudio(Constants.UIOpenPage);
		mainCitySys.OpenBuyPanel(BuyType.Coin);
	}

	public void ClickTaskBtn() {
		audioSvc.Play2DAudio(Constants.UIOpenPage);
		mainCitySys.OpenTaskPanel();
	}

	public void ClickDungeonBtn() {
		audioSvc.Play2DAudio(Constants.UIOpenPage);
		mainCitySys.EnterDungeonPanel();
	}
	#endregion
}