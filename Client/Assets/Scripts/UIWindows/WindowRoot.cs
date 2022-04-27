/****************************************************
    文件：WindowRoot.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/23 12:22:25
    功能：UI界面基类
*****************************************************/

using System;
using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour
{
    protected AudioService audioService;
    protected NetService netService;
    protected ResourceService resourceService;
    protected TimerService timerService;

    protected PlayerData playerData;

    public bool GetWindowState()
    {
        return gameObject.activeSelf;
    }

    public void SetWindowState(bool isActive = true)
    {
        if (gameObject.activeSelf != isActive) SetActive(gameObject, isActive);

        if (isActive)
            InitWindow();
        else
            ClearWindow();
    }

    protected virtual void InitWindow()
    {
        resourceService = ResourceService.instance;
        audioService = AudioService.instance;
        netService = NetService.instance;
        timerService = TimerService.instance;

        playerData = GameRoot.instance.playerData;
    }

    private void ClearWindow()
    {
        resourceService = null;
        audioService = null;
        netService = null;
    }

    #region 工具函数

    #region 状态设置

    protected void SetActive(GameObject go, bool isActive = true)
    {
        go.SetActive(isActive);
    }

    protected void SetActive(Transform trans, bool isActive = true)
    {
        trans.gameObject.SetActive(isActive);
    }

    protected void SetActive(RectTransform rect, bool isActive = true)
    {
        rect.gameObject.SetActive(isActive);
    }

    protected void SetActive(Image image, bool isActive = true)
    {
        image.gameObject.SetActive(isActive);
    }

    protected void SetActive(Text text, bool isActive = true)
    {
        text.gameObject.SetActive(isActive);
    }

    #endregion

    #region 文本设置

    protected void SetText(Text text, string context = "")
    {
        text.text = context;
    }

    protected void SetText(Text text, int num = 0)
    {
        text.text = num.ToString();
    }

    protected void SetText(Transform trans, string context = "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }

    protected void SetText(Transform trans, int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num);
    }

    #endregion

    protected void SetSprite(Image image, string path)
    {
        Sprite sprite = resourceService.LoadSprite(path, true);
        image.sprite = sprite;
    }

    private T GetComponent<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        t = t == null ? go.AddComponent<T>() : go.GetComponent<T>();
        return t;
    }

    protected Transform GetTransform(Transform trans, string targetName)
    {
        Transform targetTransform = null;
        for (int i = 0; i < trans.childCount; i++)
        {
            targetTransform = trans.Find(targetName);
            if (targetTransform != null)
            {
                return targetTransform;
            }

            if (trans.GetChild(i).childCount <= 0) continue;
            targetTransform = GetTransform(trans.GetChild(i).transform, targetName);
            if (targetTransform != null) break;
        }

        return targetTransform;
    }

    #endregion

    #region 注册点击事件

    protected void OnClickDown(GameObject go, Action<PointerEventData> callBack)
    {
        PeListener peListener = GetComponent<PeListener>(go);
        peListener.onClickDown = callBack;
    }

    protected void OnClickUp(GameObject go, Action<PointerEventData> callBack)
    {
        PeListener peListener = GetComponent<PeListener>(go);
        peListener.onClickUp = callBack;
    }

    protected void OnDrag(GameObject go, Action<PointerEventData> callBack)
    {
        PeListener peListener = GetComponent<PeListener>(go);
        peListener.onDrag = callBack;
    }

    protected void OnClick(GameObject go, Action<object> callBack, object args)
    {
        PeListener peListener = GetComponent<PeListener>(go);
        peListener.onClick = callBack;
        peListener.args = args;
    }

    #endregion
}