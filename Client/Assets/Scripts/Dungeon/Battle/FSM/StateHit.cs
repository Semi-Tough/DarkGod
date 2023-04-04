/****************************************************
    文件：StateHit.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/11/01 14:03:21
    功能：受击状态
*****************************************************/

using UnityEngine;
public class StateHit : IState {
	public void Enter(EntityBase entity, params object[] args) {
		entity.SetMoveDir(Vector2.zero);
		entity.SetBlend(Constants.BlendIdle);
		entity.curAniState = AniState.Hit;

		entity.SetSkillMove(false);
		entity.SetAniAction(Constants.ActionHit);
	}
	public void Process(EntityBase entity, params object[] args) {
		entity.RemoveSkill();

		if(entity.entityType == EntityType.Player) {
			entity.canControl = false;
			entity.canRelSkill = false;
			AudioSource audioSource = entity.GetAudio();
			AudioSvc.Instance.Play3DAudio(Constants.AssassinHit, audioSource);
		}
		TimerSvc.Instance.AddTask(entity.GetAnimationClipTime("hit"), tid => entity.Idle());
	}
	public void Exit(EntityBase entity, params object[] args) {
		entity.SetAniAction(Constants.ActionDefault);
	}
}