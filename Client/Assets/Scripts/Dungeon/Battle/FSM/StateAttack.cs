/****************************************************
	文件：StateAttack.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年04月01日 星期五 11:48   	
	功能：攻击状态
*****************************************************/

using UnityEngine;
public class StateAttack : IState {
	public void Enter(EntityBase entity, params object[] args) {
		entity.SetMoveDir(Vector2.zero);
		entity.SetBlend(Constants.BlendIdle);
		entity.curAniState = AniState.Attack;
	}

	public void Process(EntityBase entity, params object[] args) {
		if(entity.entityType == EntityType.Player) {
			entity.canRelSkill = false;
		}
		entity.SetAttack((int)args[0]);
	}

	public void Exit(EntityBase entity, params object[] args) {
		entity.SetAniAction(Constants.ActionDefault);
		entity.canControl = true;
	}
}