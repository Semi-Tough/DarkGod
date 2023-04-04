/****************************************************
    文件：EntityMonster.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/10/30 19:58:29
    功能：怪物逻辑实体
*****************************************************/

using cfg;
using UnityEngine;
public class EntityMonster : EntityBase {
	public MapMonster mapMonster;
	public MonsterConfig monsterConfig;
	
	private float atkTime;
	private float atkTimer;

	public EntityMonster() {
		entityType = EntityType.Monster;
		atkTimer = Random.Range(1f, 5f);
	}
	public override void SetBattleProps(BattleProps props) {
		int level = mapMonster.Level;
		
		BattleProps newProps = new BattleProps(
			level * props.Hp,
			level * props.Ad,
			level * props.Ap,
			level * props.Addef,
			level * props.Apdef,
			level * props.Dodge,
			level * props.Pierce,
			level * props.Critical
			);
		Props = newProps;
		hp = props.Hp;
	}

	public override void TickAILogic() {
		if(!canControl) return;
		if(curAniState is AniState.Idle or AniState.Move) {
			if(battleMgr.isPause) {
				Idle();
				return;
			}
			atkTimer += Time.deltaTime;
			if(InAttackDis()) {
				SetMoveDir(Vector2.zero);
				if(atkTimer > atkTime) {
					
					Attack(monsterConfig.SkillID);
					atkTimer = 0;
					atkTime = Random.Range(1f, 5f);
				}
				else {
					Idle();
				}
			}
			else if(curAniState is not AniState.Attack) {
				SetMoveDir(GetMoveDir());
				Move();
			}
		}
	}

	public override Vector2 GetMoveDir() {
		EntityPlayer player = battleMgr.entityPlayer;
		if(player == null || player.curAniState == AniState.Die) {
			canControl = false;
			return Vector2.zero;
		}
		else {
			Vector3 target = player.Position;
			Vector3 self = Position;
			return new Vector2(target.x - self.x, target.z - self.z).normalized;
		}
	}

	public override bool GetBreakState() {
		return monsterConfig.IsStop;
	}
	public override void SetLifeBar(int oldHp, int newHp) {
		if(monsterConfig.Type == (int)MonsterType.Normal) {
			base.SetLifeBar(oldHp, newHp);
		}
		else if(monsterConfig.Type == (int)MonsterType.Boss) {
			BattleSys.Instance.BattlePanel.SetBossHpValue(oldHp, newHp,monsterConfig.Props.Hp);
		}
	}
	private bool InAttackDis() {
		EntityPlayer player = battleMgr.entityPlayer;
		if(player == null || player.curAniState == AniState.Die) {
			canControl = false;
			return false;
		}
		else {
			Vector3 target = player.Position;
			Vector3 self = Position;
			return Vector3.Distance(target, self) < monsterConfig.AtkDis;
		}
	}
}