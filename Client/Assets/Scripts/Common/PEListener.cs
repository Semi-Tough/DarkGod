/****************************************************
    文件：PEListener
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月03日 星期四 18:21
    功能：UI事件监听
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class PEListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,
    IPointerClickHandler
{
    public object args;
    public Action<object> onClick;
    public Action<PointerEventData> onClickDown;
    public Action<PointerEventData> onClickUp;
    public Action<PointerEventData> onDrag;

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(args);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onClickDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onClickUp?.Invoke(eventData);
    }
}