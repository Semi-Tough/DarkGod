/****************************************************
	文件：StateManager.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 14:59   	
	功能：状态管理器
*****************************************************/

using PETool.PELogger;
using System.Collections.Generic;
using UnityEngine;
public enum AniState {
	None,
	Born,
	Idle,
	Move,
	Attack,
	Hit,
	Die
}
public class StateMgr : MonoBehaviour {
	private readonly Dictionary<AniState, IState> stateDic = new();

	public void Init() {
		stateDic.Add(AniState.Born, new StateBorn());
		stateDic.Add(AniState.Idle, new StateIdle());
		stateDic.Add(AniState.Move, new StateMove());
		stateDic.Add(AniState.Attack, new StateAttack());
		stateDic.Add(AniState.Hit, new StateHit());
		stateDic.Add(AniState.Die, new StateDie());
		PELogger.Log("状态管理器加载完成");
	}

	public void ChangeState(EntityBase entity, AniState targetState, params object[] args) {
		if(entity.curAniState == AniState.Die) return;
		if(entity.curAniState == targetState) return;
		if(!stateDic.ContainsKey(targetState)) return;

		if(entity.curAniState != AniState.None) {
			stateDic[entity.curAniState].Exit(entity, args);
		}

		stateDic[targetState].Enter(entity, args);
		stateDic[targetState].Process(entity, args);
	}
}