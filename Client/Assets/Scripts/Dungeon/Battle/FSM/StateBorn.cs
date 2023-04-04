/****************************************************
    文件：StateBorn.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/11/01 14:04:10
    功能：出生状态
*****************************************************/

using UnityEngine;
public class StateBorn : IState {
	public void Enter(EntityBase entity, params object[] args) {
		entity.SetMoveDir(Vector2.zero);
		entity.SetBlend(Constants.BlendIdle);
		entity.curAniState = AniState.Born;
		entity.SetAniAction(Constants.ActionBorn);
	}

	public void Process(EntityBase entity, params object[] args) {
		TimerSvc.Instance.AddTask(entity.GetAnimationClipTime("born"), tid => entity.Idle());
	}

	public void Exit(EntityBase entity, params object[] args) {
		entity.SetAniAction(Constants.ActionDefault);
	}
}