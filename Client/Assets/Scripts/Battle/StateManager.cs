/****************************************************
	文件：StateManager.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 14:59   	
	功能：状态管理器
*****************************************************/

using System.Collections.Generic;
using PEProtocol;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private readonly Dictionary<AnimationState, IState> _fsm = new Dictionary<AnimationState, IState>();

    public void Init()
    {
        _fsm.Add(AnimationState.Idle, new StateIdle());
        _fsm.Add(AnimationState.Move, new StateMove());
        _fsm.Add(AnimationState.Attack, new StateAttack());
        PeCommon.Log("状态管理器加载完成");
    }

    public void ChangeState(EntityBase entity, AnimationState targetState, params object[] args)
    {
        if (entity.currentState == targetState) return;
        if (!_fsm.ContainsKey(targetState)) return;

        if (entity.currentState != AnimationState.None)
        {
            _fsm[entity.currentState].Exit(entity, args);
        }

        _fsm[targetState].Enter(entity, args);
        _fsm[targetState].Process(entity, args);
    }
}