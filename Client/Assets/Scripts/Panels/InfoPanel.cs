/****************************************************
    文件：InfoPanel
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月07日 星期一 13:26
    功能：角色信息展示界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
public class InfoPanel : PanelRoot {
	public Text TxtInfo;
	public Text TxtExpPrg;
	public Image ImgExpPrg;
	public Text TxtPowerPrg;
	public Image ImgPowerPrg;
	public Text TxtJob;
	public Text TxtFight;
	public Text TxtHp;
	public Text TxtHurt;
	public Text TxtDefence;
	public Text DetailHp;
	public Text DetailAd;
	public Text DetailAp;
	public Text DetailAdDef;
	public Text DetailApDef;
	public Text DetailDodge;
	public Text DetailPierce;
	public Text DetailCritical;
	public RawImage RawImage;

	public RectTransform DetailProperties;
	private Vector2 startPos;

	override protected void InitPanel() {
		base.InitPanel();
		RefreshUI();
		SetActive(DetailProperties, false);
		ShowTouchEvents();
	}

	private void RefreshUI() {
		SetText(TxtInfo, playerData.Name + "Level." + playerData.Level);
		SetText(TxtExpPrg, playerData.Exp + "/" + Protocol.GetExpUpValueByLevel(playerData.Level));
		ImgExpPrg.fillAmount =
			playerData.Exp * 1.0f / Protocol.GetExpUpValueByLevel(playerData.Level);
		SetText(TxtPowerPrg, playerData.Power + "/" + Protocol.GetPowerLimit(playerData.Level));
		ImgPowerPrg.fillAmount = playerData.Power * 1.0f / Protocol.GetPowerLimit(playerData.Level);

		SetText(TxtJob, "暗夜刺客");
		SetText(TxtFight, Protocol.GetFightByProperty(playerData));
		SetText(TxtHp, playerData.Hp);
		SetText(TxtHurt, playerData.Ad + playerData.Ap);
		SetText(TxtDefence, playerData.AdDef + playerData.ApDef);

		//DetailedProperties
		SetText(DetailHp, playerData.Hp);
		SetText(DetailAd, playerData.Ad);
		SetText(DetailAp, playerData.Ap);
		SetText(DetailAdDef, playerData.AdDef);
		SetText(DetailApDef, playerData.ApDef);
		SetText(DetailDodge, playerData.Dodge);
		SetText(DetailPierce, playerData.Pierce);
		SetText(DetailCritical, playerData.Critical);
	}

	private void ShowTouchEvents() {
		OnClickDown(RawImage.gameObject, data => {
			startPos = data.position;
			MainCitySys.Instance.SetStartRotate();
		});

		OnDrag(RawImage.gameObject, data => {
			float rotate = data.position.x - startPos.x;
			MainCitySys.Instance.SetPlayerRotate(rotate);
		});
	}

	#region 点击事件
	public void ClickCloseBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		MainCitySys.Instance.CloseInfoPanel();
	}

	public void ClickDetailedPropertiesBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		SetActive(DetailProperties);
	}

	public void ClickCloseDetailedPropertiesBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		SetActive(DetailProperties, false);
	}
	#endregion
}