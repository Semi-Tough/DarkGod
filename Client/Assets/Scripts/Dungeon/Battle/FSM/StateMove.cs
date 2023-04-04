/****************************************************
	文件：StateMove.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 12:17   	
	功能：移动状态
*****************************************************/

public class StateMove : IState {
	public void Enter(EntityBase entity, params object[] args) {
		entity.SetBlend(Constants.BlendMove);
		entity.curAniState = AniState.Move;

	}

	public void Process(EntityBase entity, params object[] args) {
	}

	public void Exit(EntityBase entity, params object[] args) {
		entity.SetAniAction(Constants.ActionDefault);

	}
}