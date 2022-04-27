/****************************************************
    文件：InfoWindow
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月07日 星期一 13:26
    功能：角色信息展示界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : WindowRoot
{
    public Text txtInfo;
    public Text txtExpPrg;
    public Image imgExpPrg;
    public Text txtPowerPrg;
    public Image imgPowerPrg;

    public Text txtJob;
    public Text txtFight;
    public Text txtHp;
    public Text txtHurt;
    public Text txtDefence;

    public Text detailHp;
    public Text detailAd;
    public Text detailAp;
    public Text detailAdDef;
    public Text detailApDef;
    public Text detailDodge;
    public Text detailPierce;
    public Text detailCritical;

    public RawImage rawImage;
    public Transform detailedProperties;
    private Vector2 _startPos;

    protected override void InitWindow()
    {
        base.InitWindow();
        RefreshUI();
        ShowTouchEvents();
    }

    private void RefreshUI()
    {
        SetText(txtInfo, playerData.Name + "Level." + playerData.Level);
        SetText(txtExpPrg, playerData.Exp + "/" + PeCommon.GetExpUpValueByLevel(playerData.Level));
        imgExpPrg.fillAmount =
            playerData.Exp * 1.0f / PeCommon.GetExpUpValueByLevel(playerData.Level);
        SetText(txtPowerPrg, playerData.Power + "/" + PeCommon.GetPowerLimit(playerData.Level));
        imgPowerPrg.fillAmount = playerData.Power * 1.0f / PeCommon.GetPowerLimit(playerData.Level);

        SetText(txtJob, "暗夜刺客");
        SetText(txtFight, PeCommon.GetFightByProperty(playerData));
        SetText(txtHp, playerData.Hp);
        SetText(txtHurt, playerData.Ad + playerData.Ap);
        SetText(txtDefence, playerData.AdDef + playerData.ApDef);

        //DetailedProperties
        SetText(detailHp, playerData.Hp);
        SetText(detailAd, playerData.Ad);
        SetText(detailAp, playerData.Ap);
        SetText(detailAdDef, playerData.AdDef);
        SetText(detailApDef, playerData.ApDef);
        SetText(detailDodge, playerData.Dodge);
        SetText(detailPierce, playerData.Pierce);
        SetText(detailCritical, playerData.Critical);
    }

    private void ShowTouchEvents()
    {
        OnClickDown(rawImage.gameObject, data =>
        {
            _startPos = data.position;
            MainCitySystem.Instance.SetStartRotate();
        });

        OnDrag(rawImage.gameObject, data =>
        {
            float rotate = data.position.x - _startPos.x;
            MainCitySystem.Instance.SetPlayerRotate(rotate);
        });
    }

    #region 点击事件

    public void ClickCloseBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        MainCitySystem.Instance.CloseInfoWindow();
    }

    public void ClickDetailedPropertiesBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        SetActive(detailedProperties);
    }

    public void ClickCloseDetailedPropertiesBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        SetActive(detailedProperties, false);
    }

    #endregion
}