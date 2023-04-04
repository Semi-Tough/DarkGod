/****************************************************
	文件：BattlePanel.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 19:37   	
	功能：战斗控制界面
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattlePanel : PanelRoot {
	public Image ImgTouch;
	public Image ImgDirBg;
	public Image ImgDirPoint;

	public Button BtnHead;
	public Image ImgHP;
	public Text TxtHP;
	public Text TxtLevel;
	public Text TxtName;

	public Text TxtExpProgress;
	public GridLayoutGroup ExpImgGroup;
	public Transform HUDRoot;

	public Animation playerDodgeAni;
	private BattleSys battleSys;
	private float pointDistance;
	private Vector2 startPosition;
	private Dictionary<string, HUDItem> hudItemDic;
	public Vector2 MoveDir { get; private set; }

	override protected void InitPanel() {
		base.InitPanel();
		battleSys = BattleSys.Instance;
		hudItemDic = new Dictionary<string, HUDItem>();

		skill1CdTime = resSvc.GetSkillConfig(101).CdTime * 1.0f / 1000;
		skill2CdTime = resSvc.GetSkillConfig(102).CdTime * 1.0f / 1000;
		skill3CdTime = resSvc.GetSkillConfig(103).CdTime * 1.0f / 1000;
		SetActive(ImgCd1, false);
		SetActive(ImgCd2, false);
		SetActive(ImgCd3, false);
		isSkill1Cd = false;
		isSkill2Cd = false;
		isSkill3Cd = false;

		RegisterTouchEvents();
		RefreshUI();
		SetBossLifeBar(false);
	}

	private void Update() {
		#region SkillCD
		float delta = Time.deltaTime;
		if(isSkill1Cd) {
			skill1Timer += delta;
			if(skill1Timer >= skill1CdTime) {
				SetActive(ImgCd1, false);
				isSkill1Cd = false;
				skill1Timer = 0;
				timer1 = 0;
			}
			else {
				ImgCd1.fillAmount = 1 - skill1Timer / skill1CdTime;
			}

			timer1 += delta;
			if(timer1 >= 1) {
				timer1 -= 1;
				skill1CdTxt -= 1;
				SetText(TxtCd1, skill1CdTxt);
			}
		}

		if(isSkill2Cd) {
			skill2Timer += delta;
			if(skill2Timer >= skill2CdTime) {
				SetActive(ImgCd2, false);
				isSkill2Cd = false;
				skill2Timer = 0;
				timer2 = 0;
			}
			else {
				ImgCd2.fillAmount = 1 - skill2Timer / skill2CdTime;
			}

			timer2 += delta;
			if(timer2 >= 1) {
				timer2 -= 1;
				skill2CdTxt -= 1;
				SetText(TxtCd2, skill2CdTxt);
			}
		}

		if(isSkill3Cd) {
			skill3Timer += delta;
			if(skill3Timer >= skill3CdTime) {
				SetActive(ImgCd3, false);
				isSkill3Cd = false;
				skill3Timer = 0;
				timer3 = 0;
			}
			else {
				ImgCd3.fillAmount = 1 - skill3Timer / skill3CdTime;
			}

			timer3 += delta;
			if(timer3 >= 1) {
				timer3 -= 1;
				skill3CdTxt -= 1;
				SetText(TxtCd3, skill3CdTxt);
			}
		}
		#endregion

		if(BossLifeBar.gameObject.activeSelf) {
			UpdateBossLifeBar();
		}
	}

	private void RefreshUI() {
		playerMaxHp = playerData.Hp;
		SetText(TxtName, playerData.Name);
		SetText(TxtLevel, playerData.Level);
		SetText(TxtHP, playerData.Hp + "/" + playerData.Hp);
		ImgHP.fillAmount = 1;

		#region 进度条UI设置
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
	}

	private void RegisterTouchEvents() {
		SetActive(ImgDirPoint, false);
		pointDistance = 1.0f *
		                Screen.height /
		                Constants.ScreenStandardHeight *
		                Constants.RockerOperateDistance;

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

			MoveDir = dir.normalized;
			battleSys.SetPlayerMove(MoveDir);
		});

		OnClickUp(ImgTouch.gameObject, eventData => {
			ImgDirBg.rectTransform.localPosition = Vector3.zero;
			ImgDirPoint.rectTransform.localPosition = Vector3.zero;
			SetActive(ImgDirPoint, false);
			MoveDir = Vector2.zero;
			battleSys.SetPlayerMove(MoveDir);
		});
	}

	public void ClickBtnHead() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		battleSys.SetEndPanelState(EndType.Quit);
		battleSys.BattleMgr.isPause = true;
	}

	#region PlayerHUD
	private int playerMaxHp;
	public void SetPlayerDodge() {
		playerDodgeAni.Stop();
		playerDodgeAni.Play();
	}
	public void SetPlayerHp(int hp) {
		SetText(TxtHP, $"{hp.ToString()}/{playerMaxHp.ToString()}");
		ImgHP.fillAmount = hp * 1.0f / playerMaxHp;
	}
	#endregion

	#region MonsterHUD
	public void AddMonsterHUDItem(string name, Transform trans, int hp) {
		if(hudItemDic.TryGetValue(name, out HUDItem hudItem)) {
			hudItem.gameObject.SetActive(true);
			hudItem.InitHUDInfo(trans, hp);
			return;
		}

		GameObject go = Instantiate(resSvc.LoadPrefab(PathDefine.HUDItem), HUDRoot, true);
		hudItem = go.GetComponent<HUDItem>();
		hudItem.InitHUDInfo(trans, hp);
		hudItemDic.Add(name, hudItem);
	}

	public void RemoveMonsterHUDItem(string name) {
		if(hudItemDic.TryGetValue(name, out HUDItem hudItem)) {
			Destroy(hudItem.gameObject);
			hudItemDic.Remove(name);
		}
	}

	public void RemoveMonsterHUDItemAll() {
		foreach(KeyValuePair<string, HUDItem> hudItem in hudItemDic) {
			Destroy(hudItem.Value.gameObject);
		}
		hudItemDic.Clear();
	}

	public void SetMonsterDodge(string name) {
		if(hudItemDic.TryGetValue(name, out HUDItem hudItem)) {
			hudItem.SetDodge();
		}
	}

	public void SetMonsterCritical(string name, int damage) {
		if(hudItemDic.TryGetValue(name, out HUDItem hudItem)) {
			hudItem.SetCritical(damage);
		}
	}

	public void SetMonsterDamage(string name, int damage) {
		if(hudItemDic.TryGetValue(name, out HUDItem hudItem)) {
			hudItem.SetDamage(damage);
		}
	}

	public void SetMonsterLifeBar(string name, int oldHp, int newHp) {
		if(hudItemDic.TryGetValue(name, out HUDItem hudItem)) {
			hudItem.SetLifeBar(oldHp, newHp);
		}
	}
	#endregion

	#region BossHUD
	public Transform BossLifeBar;
	public Text txtBossInfo;
	public Image ImgHpMiddle;
	public Image ImgHpTop;
	public void SetBossLifeBar(bool show = true, float prg = 1f) {
		BossLifeBar.gameObject.SetActive(show);
		ImgHpTop.fillAmount = prg;
		ImgHpMiddle.fillAmount = prg;
	}
	private float currentPrg;
	private float targetPrg;
	public void SetBossHpValue(int oldHp, int newHp, int maxHp) {
		currentPrg = oldHp * 1.0f / maxHp;
		targetPrg = newHp * 1.0f / maxHp;
		ImgHpTop.fillAmount = targetPrg;
		if(targetPrg == 0) {
			SetBossLifeBar(false);
		}
	}
	private void UpdateBossLifeBar() {
		float smoothValue = Constants.AccLifeBar * Time.deltaTime;
		if(Mathf.Abs(currentPrg - targetPrg) < smoothValue) {
			currentPrg = targetPrg;
		}
		else if(currentPrg > targetPrg) {
			currentPrg -= smoothValue;
		}
		else {
			currentPrg += smoothValue;
		}

		ImgHpMiddle.fillAmount = currentPrg;
	}
	#endregion

	#region Attack
	public void NormalAttack() {
		battleSys.SetPlayerAttack(0);
	}

	public Image ImgCd1;
	public Text TxtCd1;
	private bool isSkill1Cd;
	private int skill1CdTxt;
	private float skill1CdTime;
	private float skill1Timer;
	private float timer1;

	public void Skill1() {
		if(isSkill1Cd || !CanRelSkill()) return;
		battleSys.SetPlayerAttack(1);
		isSkill1Cd = true;
		SetActive(ImgCd1);
		ImgCd1.fillAmount = 1;
		skill1CdTxt = (int)skill1CdTime;
		SetText(TxtCd1, skill1CdTxt);
	}

	public Image ImgCd2;
	public Text TxtCd2;
	private bool isSkill2Cd;
	private int skill2CdTxt;
	private float skill2CdTime;
	private float skill2Timer;
	private float timer2;

	public void Skill2() {
		if(isSkill2Cd || !CanRelSkill()) return;
		battleSys.SetPlayerAttack(2);
		isSkill2Cd = true;
		SetActive(ImgCd2);
		ImgCd2.fillAmount = 1;
		skill2CdTxt = (int)skill2CdTime;
		SetText(TxtCd2, skill2CdTxt);
	}

	public Image ImgCd3;
	public Text TxtCd3;
	private bool isSkill3Cd;
	private int skill3CdTxt;
	private float skill3CdTime;
	private float skill3Timer;
	private float timer3;

	public void Skill3() {
		if(isSkill3Cd || !CanRelSkill()) return;
		battleSys.SetPlayerAttack(3);
		isSkill3Cd = true;
		SetActive(ImgCd3);
		ImgCd3.fillAmount = 1;
		skill3CdTxt = (int)skill3CdTime;
		SetText(TxtCd3, skill3CdTxt);
	}

	private bool CanRelSkill() {
		return battleSys.BattleMgr.CanRelSkill();
	}
	#endregion
}