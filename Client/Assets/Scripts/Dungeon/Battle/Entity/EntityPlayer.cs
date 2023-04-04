/****************************************************
	文件：EntityPlayer.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 13:24   	
	功能：角色逻辑实体控制器
*****************************************************/

using UnityEngine;
public class EntityPlayer : EntityBase
{
	public EntityPlayer() {
		entityType = EntityType.Player;
	}
	public override Vector2 GetMoveDir()
	{
		return battleMgr.GetMoveDir();
	}

	public override void SetDodge() {
		BattleSys.Instance.BattlePanel.SetPlayerDodge();
	}
	public override void SetLifeBar(int oldHp, int newHp) {
		BattleSys.Instance.BattlePanel.SetPlayerHp(newHp);
	}
}