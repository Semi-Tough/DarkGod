/****************************************************
    文件：LoopDragonAnimation.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/23 14:49:55
    功能：飞龙循环动画
*****************************************************/

using UnityEngine;
public class LoopDragonAnimation : MonoBehaviour
{
    private Animation dragonAni;

    private void Awake()
    {
        dragonAni = GetComponent<Animation>();
    }

    protected void Start()
    {
        if (dragonAni != null) InvokeRepeating(nameof(PlayDragonAnimation), 0, 20);
    }

    private void PlayDragonAnimation()
    {
        if (dragonAni != null) dragonAni.Play();
    }
}