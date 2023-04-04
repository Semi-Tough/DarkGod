/****************************************************
	文件：StateIdle.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 12:16   	
	功能：待机状态
*****************************************************/

using UnityEngine;
public class StateIdle : IState {
	public void Enter(EntityBase entity, params object[] args) {
		entity.SetMoveDir(Vector2.zero);
		entity.SetBlend(Constants.BlendIdle);
		entity.curAniState = AniState.Idle;
	}

	public void Process(EntityBase entity, params object[] args) {
		entity.entityState = EntityState.None;
		entity.skillEndCB = -1;
		entity.canCombo = true;
		entity.canControl = true;
		entity.canRelSkill = true;
	}

	public void Exit(EntityBase entity, params object[] args) {
		entity.SetAniAction(Constants.ActionDefault);
	}
}