/****************************************************
	文件：EntityBase.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 12:13   	
	功能：逻辑实体的基类
*****************************************************/

using UnityEngine;

public abstract class EntityBase
{
    public AnimationState currentState = AnimationState.None;

    public BattleManager battleManager;
    public StateManager stateManager;
    public SkillManager skillManager;
    public Controller controller;
    public bool canControl = true;

    public void Idle()
    {
        stateManager.ChangeState(this, AnimationState.Idle);
    }

    public void Move()
    {
        stateManager.ChangeState(this, AnimationState.Move);
    }

    public void Attack(int skillId)
    {
        stateManager.ChangeState(this, AnimationState.Attack, skillId);
    }

    //处理技能特效
    public virtual void SetEffect(int skillId)
    {
        skillManager.SetAttackEffect(this, skillId);
    }

    public virtual void SetEffectState(string effectName, float delay)
    {
        if (controller != null)
        {
            controller.SetEffectState(effectName, delay);
        }
    }

    public virtual void SetSkillMoveState(bool move, float speed = 0f)
    {
        if (controller != null)
        {
            controller.SetSkillMoveState(move, speed);
        }
    }

    public virtual Vector2 GetInPutDir()
    {
        return Vector2.zero;
    }

    #region 动画事件

    public virtual void SetBleed(float blend)
    {
        if (controller != null)
        {
            controller.SetBlend(blend);
        }
    }

    public virtual void SetDirection(Vector2 dir)
    {
        if (controller != null)
        {
            controller.CharacterDir = dir;
        }
    }

    public virtual void SetAction(int index)
    {
        if (controller != null)
        {
            controller.SetAction(index);
        }
    }

    #endregion
}