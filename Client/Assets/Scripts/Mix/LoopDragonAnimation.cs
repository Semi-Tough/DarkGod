/****************************************************
    文件：LoopDragonAnimation.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/23 14:49:55
    功能：飞龙循环动画
*****************************************************/

using UnityEngine;

public class LoopDragonAnimation : MonoBehaviour
{
    private Animation _animation;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    protected void Start()
    {
        if (_animation != null) InvokeRepeating(nameof(PlayDragonAnimation), 0, 20);
    }

    private void PlayDragonAnimation()
    {
        if (_animation != null) _animation.Play();
    }
}