/****************************************************
    文件：PanelRoot.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/23 12:22:25
    功能：UI界面基类
*****************************************************/

using PEProtocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PanelRoot : MonoBehaviour {
	protected AudioSvc audioSvc;
	protected NetSvc netSvc;
	protected ResSvc resSvc;
	protected TimerSvc timerSvc;

	protected PlayerData playerData;

	private void OnValidate() {
		BindingUI();
	}

	private void BindingUI() {
		Dictionary<string, List<UIBehaviour>> uiBehaviorDic = new();

		UIBehaviour[] behaviours = GetComponentsInChildren<UIBehaviour>();
		for(int i = 0; i < behaviours.Length; i++) {
			string uiName = behaviours[i].gameObject.name.ToLower();
			if(uiBehaviorDic.ContainsKey(uiName)) {
				uiBehaviorDic[uiName].Add(behaviours[i]);
			}
			else {
				uiBehaviorDic.Add(uiName, new List<UIBehaviour>{ behaviours[i] });
			}
		}

		Type panelType = GetType();
		FieldInfo[] fieldInfos = panelType.GetFields(
			BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		foreach(FieldInfo fieldInfo in fieldInfos) {
			string key = fieldInfo.Name.ToLower();
			if(!uiBehaviorDic.ContainsKey(key)) continue;

			foreach(UIBehaviour behaviour in uiBehaviorDic[key]) {
				if(fieldInfo.FieldType == behaviour.GetType()) {
					fieldInfo.SetValue(this, behaviour);
				}
			}
		}
	}


	public bool GetPanelState() {
		return gameObject.activeSelf;
	}

	public void SetPanelState(bool isActive = true) {
		if(gameObject.activeSelf != isActive) SetActive(gameObject, isActive);

		if(isActive)
			InitPanel();
		else
			ClearPanel();
	}

	protected virtual void InitPanel() {
		resSvc = ResSvc.Instance;
		audioSvc = AudioSvc.Instance;
		netSvc = NetSvc.Instance;
		timerSvc = TimerSvc.Instance;
		playerData = GameRoot.Instance.PlayerData;
	}

	private void ClearPanel() {
		resSvc = null;
		audioSvc = null;
		netSvc = null;
	}

	#region 工具函数
	#region 状态设置
	protected void SetActive(GameObject go, bool isActive = true) {
		go.SetActive(isActive);
	}

	protected void SetActive(Transform trans, bool isActive = true) {
		trans.gameObject.SetActive(isActive);
	}

	protected void SetActive(RectTransform rect, bool isActive = true) {
		rect.gameObject.SetActive(isActive);
	}

	protected void SetActive(Image image, bool isActive = true) {
		image.gameObject.SetActive(isActive);
	}

	protected void SetActive(Text text, bool isActive = true) {
		text.gameObject.SetActive(isActive);
	}
	#endregion

	#region 文本设置
	protected void SetText(Text text, string context = "") {
		text.text = context;
	}

	protected void SetText(Text text, int num = 0) {
		text.text = num.ToString();
	}

	protected void SetText(Transform trans, string context = "") {
		SetText(trans.GetComponent<Text>(), context);
	}

	protected void SetText(Transform trans, int num = 0) {
		SetText(trans.GetComponent<Text>(), num);
	}
	#endregion

	protected void SetSprite(Image image, string path) {
		Sprite sprite = resSvc.LoadSprite(path, true);
		image.sprite = sprite;
	}

	private T GetComponent<T>(GameObject go) where T : Component {
		T t = go.GetComponent<T>();
		t = t == null ? go.AddComponent<T>() : go.GetComponent<T>();
		return t;
	}

	protected Transform GetTransform(Transform trans, string targetName) {
		Transform targetTransform = null;
		for(int i = 0; i < trans.childCount; i++) {
			targetTransform = trans.Find(targetName);
			if(targetTransform != null) {
				return targetTransform;
			}

			if(trans.GetChild(i).childCount <= 0) continue;
			targetTransform = GetTransform(trans.GetChild(i).transform, targetName);
			if(targetTransform != null) break;
		}

		return targetTransform;
	}
	#endregion

	#region 注册点击事件
	protected void OnClickDown(GameObject go, Action<PointerEventData> callBack) {
		PEListener peListener = GetComponent<PEListener>(go);
		peListener.onClickDown = callBack;
	}

	protected void OnClickUp(GameObject go, Action<PointerEventData> callBack) {
		PEListener peListener = GetComponent<PEListener>(go);
		peListener.onClickUp = callBack;
	}

	protected void OnDrag(GameObject go, Action<PointerEventData> callBack) {
		PEListener peListener = GetComponent<PEListener>(go);
		peListener.onDrag = callBack;
	}

	protected void OnClick(GameObject go, Action<object> callBack, object args) {
		PEListener peListener = GetComponent<PEListener>(go);
		peListener.onClick = callBack;
		peListener.args = args;
	}
	#endregion
}