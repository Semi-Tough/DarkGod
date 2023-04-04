/****************************************************
    文件：StateDie.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/11/01 14:03:44
    功能：死亡状态
*****************************************************/

using UnityEngine;
public class StateDie : IState {
	public void Enter(EntityBase entity, params object[] args) {
		entity.SetMoveDir(Vector2.zero);
		entity.SetBlend(Constants.BlendIdle);
		entity.curAniState = AniState.Die;
		entity.SetAniAction(Constants.ActionDie);
	}

	public void Process(EntityBase entity, params object[] args) {
		entity.RemoveSkill();

		if(entity.entityType == EntityType.Player) {
			entity.canControl = false;
			entity.canRelSkill = false;
			entity.battleMgr.EndBattle(false,entity.Hp);
		}
		else if(entity.entityType == EntityType.Monster) {
			entity.battleMgr.RemoveDeadMonster(entity.Name);
			entity.controller.Character.enabled = false;
			TimerSvc.Instance.AddTask(Constants.DieAniLength, tid => {
				entity.SetActive(false);
			});
		}
	}

	public void Exit(EntityBase entity, params object[] args) {
		entity.SetAniAction(Constants.ActionDefault);
	}
}