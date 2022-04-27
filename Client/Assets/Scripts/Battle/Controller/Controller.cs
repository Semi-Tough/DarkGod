/****************************************************
	文件：Controller.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 13:52   	
	功能：表现实体控制器基类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    protected TimerService timerService;
    
    public Animator animator;
    private static readonly int Blend = Animator.StringToHash("Blend");
    private static readonly int Action = Animator.StringToHash("Action");
    protected readonly Dictionary<string, GameObject> effectsDic = new Dictionary<string, GameObject>();

    protected float currentBlend;
    protected float targetBlend;
    protected float skillMoveSpeed;
    
    protected bool isMove;
    protected bool skillMove;
    
    protected Vector2 characterDir;

    public Vector2 CharacterDir
    {
        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
                SetBlend(Constants.BlendIdle);
            }
            else
            {
                isMove = true;
                SetBlend(Constants.BlendMove);
            }

            characterDir = value;
        }
    }


    public virtual void Init()
    {
        timerService=TimerService.instance;
    }
    
    public virtual void SetEffectState(string effectName, float delay)
    {
    }

    public void SetSkillMoveState(bool move, float skillSpeed = 0)
    {
        skillMove = move;
        skillMoveSpeed = skillSpeed;
    }
    
    #region 动画事件

    public virtual void SetBlend(float blend)
    {
        animator.SetFloat(Blend, blend);
    }

    protected void UpdateMixBlend()
    {
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AcceleratedSpeed * Time.deltaTime)
            currentBlend = targetBlend;
        else if (currentBlend < targetBlend)
            currentBlend += Constants.AcceleratedSpeed * Time.deltaTime;
        else
            currentBlend -= Constants.AcceleratedSpeed * Time.deltaTime;

        animator.SetFloat(Blend, currentBlend);
    }

    public virtual void SetAction(int index)
    {
        animator.SetInteger(Action, index);
    }

    #endregion
}