/****************************************************
	文件：StateAttack.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年04月01日 星期五 11:48   	
	功能：攻击状态
*****************************************************/

public class StateAttack : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AnimationState.Attack;
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetEffect((int) args[0]);
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constants.ActionDefault);
        entity.canControl = true;
    }
}