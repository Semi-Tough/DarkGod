using cfg;
using UnityEngine;
using UnityEngine.UI;
public enum EndType {
	None,
	Victory,
	Defeat,
	Quit
}
public class EndPanel : PanelRoot {
	public Transform victoryTran;
	public Transform defeatTran;
	public Transform quitTran;

	public Button BtnDefeatSure;
	public Button BtnVictorySure;
	public Button BtnQuitSure;
	public Button BtnQuitCancel;

	public Text TxtInfo1;
	public Text TxtInfo2;
	public Text TxtInfo3;

	public Animation endAnimation;
	private EndType endType = EndType.None;
	override protected void InitPanel() {
		base.InitPanel();
		RefreshUI();
	}

	private void RefreshUI() {
		switch(endType) {
			case EndType.Victory:
				SetActive(victoryTran, false);
				SetActive(defeatTran, false);
				SetActive(quitTran, false);
				break;
			case EndType.Defeat:
				SetActive(defeatTran);
				SetActive(victoryTran, false);
				SetActive(quitTran, false);
				audioSvc.Play2DAudio(Constants.BattleDefeat);
				break;
			case EndType.Quit:
				SetActive(quitTran);
				SetActive(victoryTran, false);
				SetActive(defeatTran, false);
				break;
		}
	}
	public void SetEndType(EndType endType) {
		this.endType = endType;
	}
	public void SetVictoryData(int dungeonId, int costTime, int restHp) {
		MapConfig map = resSvc.GetMapConfig(dungeonId);
		int min = costTime / 60;
		int sec = costTime % 60;
		SetText(TxtInfo1, $"通关时间: {min.ToString()}:{sec.ToString()}");
		SetText(TxtInfo2, $"剩余血量: {restHp.ToString()}");
		SetText(TxtInfo3, $"关卡奖励: {map.Coin.ToString()}金币 {map.Exp.ToString()}经验 {map.Crystal.ToString()}水晶");
		TimerSvc.Instance.AddTask(1000, tid => {
			SetActive(victoryTran);
			TimerSvc.Instance.AddTask(100, tid1 => {
				audioSvc.Play2DAudio(Constants.BattleVictoryItem, 1);
				TimerSvc.Instance.AddTask(250, tid2 => {
					audioSvc.Play2DAudio(Constants.BattleVictoryItem, 1);
					TimerSvc.Instance.AddTask(250, tid3 => {
						audioSvc.Play2DAudio(Constants.BattleVictoryItem, 1);
						TimerSvc.Instance.AddTask(300, tid4 => {
							audioSvc.Play2DAudio(Constants.BattleVictoryLogo, 0.2f);
						});
					});
				});
			});
		});
	}


	public void ClickBtnQuitSure() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		BattleSys.Instance.EndBattleSystem();
		SetPanelState(false);
		MainCitySys.Instance.LoadingMainCitySystem();
	}
	public void ClickBtnQuitCancel() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		BattleSys.Instance.BattleMgr.isPause = false;
		SetPanelState(false);
	}
}