/****************************************************
    文件：HpItem.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/11/01 19:48:22
    功能：血条物体
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
public class HUDItem : PanelRoot {
	public Image ImgLifeBarFg;
	public Image ImgLifeBarMiddle;
	public Text TxtDodge;
	public Text TxtCritical;
	public Text TxtDamage;

	public Animation DodgeAni;
	public Animation CriticalAni;
	public Animation DamageAni;
	private int maxHp;

	private Transform rootTrans;
	private RectTransform rectTrans;
	private float scaleRate;
	private Camera mainCamera;
	private float currentPrg;
	private float targetPrg;

	private void Update() {
		Vector3 screenPos = mainCamera.WorldToScreenPoint(rootTrans.position);
		rectTrans.anchoredPosition = screenPos * scaleRate;
		UpdateLifeBar();
	}

	public void InitHUDInfo(Transform trans, int hp) {
		rootTrans = trans;
		mainCamera = Camera.main;
		rectTrans = transform.GetComponent<RectTransform>();
		scaleRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;

		maxHp = hp;
		ImgLifeBarFg.fillAmount = 1;
		ImgLifeBarMiddle.fillAmount = 1;
	}

	public void SetDodge() {
		DodgeAni.Stop();
		TxtDodge.text = "闪避";
		DodgeAni.Play();
	}

	public void SetCritical(int damage) {
		CriticalAni.Stop();
		TxtCritical.text = $"暴击{damage.ToString()}";
		CriticalAni.Play();
	}

	public void SetDamage(int damage) {
		DamageAni.Stop();
		TxtDamage.text = $"-{damage.ToString()}";
		DamageAni.Play();
	}

	public void SetLifeBar(int oldHp, int newHp) {
		currentPrg = oldHp * 1.0f / maxHp;
		targetPrg = newHp * 1.0f / maxHp;
		ImgLifeBarFg.fillAmount = targetPrg;
	}

	private void UpdateLifeBar() {
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

		ImgLifeBarMiddle.fillAmount = currentPrg;
	}
}