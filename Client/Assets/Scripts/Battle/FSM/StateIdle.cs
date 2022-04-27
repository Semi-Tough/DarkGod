/****************************************************
	文件：StateIdle.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 12:16   	
	功能：待机状态
*****************************************************/

using UnityEngine;

public class StateIdle : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentState = AnimationState.Idle;
        entity.SetDirection(Vector2.zero);
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.GetInPutDir() != Vector2.zero)
        {
            entity.Move();
            entity.SetDirection(entity.GetInPutDir());
        }
        else
        {
            entity.SetBleed(Constants.BlendIdle);
        }
    }

    public void Exit(EntityBase entity, params object[] args)
    {
    }
}