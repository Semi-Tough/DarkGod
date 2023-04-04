/****************************************************
	文件：SkillManager.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 15:00   	
	功能：技能管理器
*****************************************************/

using cfg;
using PETool.PELogger;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SkillMgr : MonoBehaviour {
	private ResSvc resSvc;
	private TimerSvc timerSvc;

	public void Init() {
		resSvc = ResSvc.Instance;
		timerSvc = TimerSvc.Instance;
		PELogger.Log("技能管理器加载完成");
	}

	public void SetAttack(EntityBase attacker, int skillId) {
		SkillConfig skillConfig = resSvc.GetSkillConfig(skillId);

		attacker.SetAniAction(skillConfig.AniAction);
		attacker.SetAttackFX(skillConfig.FxName, (uint)skillConfig.SkillTime);

		attacker.skillMoveCBList.Clear();
		attacker.skillDamageCBList.Clear();

		SetAttackState(attacker, skillConfig);

		SetAttackDir(attacker);
		SetAttackMove(attacker, skillConfig);
		SetAttackDamage(attacker, skillConfig);
	}

	private void SetAttackDir(EntityBase attacker) {
		if(attacker.GetMoveDir() == Vector2.zero) {
			Vector2 dir = GetAttackDir(attacker);
			if(dir != Vector2.zero) {
				attacker.SetAttackDir(dir);
			}
		}
		else {
			attacker.SetAttackDir(attacker.GetMoveDir(), true);
		}
	}

	private Vector2 GetAttackDir(EntityBase attacker) {
		EntityBase target = null;
		if(attacker.entityType == EntityType.Player) {
			float minDis = 0;
			List<EntityMonster> monsterList = attacker.battleMgr.GetEntityMonsters();
			foreach(EntityMonster entityMonster in monsterList) {
				if(minDis == 0) {
					target = entityMonster;
					minDis = Vector3.Distance(attacker.Position, entityMonster.Position);
				}
				else if(Vector3.Distance(attacker.Position, entityMonster.Position) < minDis) {
					target = entityMonster;
					minDis = Vector3.Distance(attacker.Position, entityMonster.Position);
				}
			}
		}
		else {
			target = BattleSys.Instance.BattleMgr.entityPlayer;
		}

		if(target == null) return Vector2.zero;
		Vector3 attackerPos = attacker.Position;
		Vector3 targetPos = target.Position;
		return new Vector2(targetPos.x - attackerPos.x, targetPos.z - attackerPos.z).normalized;
	}

	private void SetAttackState(EntityBase attacker, SkillConfig skillConfig) {
		
		if(attacker.entityType == EntityType.Player) {
			Physics.IgnoreLayerCollision(7, 8, false);
			if(!skillConfig.IsCollide) {
				Physics.IgnoreLayerCollision(7, 8, true);
				timerSvc.AddTask((uint)skillConfig.SkillTime, tid => {
					Physics.IgnoreLayerCollision(7, 8, false);
				});
			}
		}


		if(!skillConfig.IsBreak) {
			attacker.entityState = EntityState.BaTi;
		}
		attacker.skillEndCB = timerSvc.AddTask((uint)skillConfig.SkillTime, tid => {
			attacker.Idle();
		});
	}

	private void SetAttackMove(EntityBase attacker, SkillConfig skillConfig) {
		attacker.canControl = false;
		attacker.SetMoveDir(Vector2.zero);
		int[] skillMoveList = skillConfig.SkillMoveList;
		uint sumTime = 0;
		foreach(int skillMoveId in skillMoveList) {
			SkillMoveConfig skillMoveConfig = resSvc.GetSkillMoveConfig(skillMoveId);
			float skillMoveSpeed = skillMoveConfig.MoveDis / ((float)skillMoveConfig.MoveTime / 1000);
			sumTime += (uint)skillMoveConfig.DelayTime;

			if(sumTime > 0) {
				int moveID = timerSvc.AddTask(sumTime, tid => {
					attacker.SetSkillMove(true, skillMoveSpeed);
					attacker.RemoveSkillMove(tid);
				});
				attacker.skillMoveCBList.Add(moveID);
			}
			else {
				attacker.SetSkillMove(true, skillMoveSpeed);
			}

			sumTime += (uint)skillMoveConfig.MoveTime;
			int stopID = timerSvc.AddTask(sumTime, tid => {
				attacker.SetSkillMove(false);
				attacker.RemoveSkillMove(tid);
			});
			attacker.skillMoveCBList.Add(stopID);
		}
	}

	private void SetAttackDamage(EntityBase attacker, SkillConfig skillConfig) {
		int[] skillActionList = skillConfig.SkillRangeList;
		uint sumTime = 0;
		for(int i = 0; i < skillActionList.Length; i++) {
			int skillActionId = skillActionList[i];
			int index = i;
			SkillRangeConfig skillRangeConfig = resSvc.GetSkillRangeConfig(skillActionId);
			sumTime += (uint)skillRangeConfig.DelayTime;
			if(sumTime > 0) {
				int damageID = timerSvc.AddTask(sumTime, tid => {
					SetDamage(skillRangeConfig, index);
					attacker.RemoveSkillDamage(tid);
				});
				attacker.skillDamageCBList.Add(damageID);
			}
			else {
				SetDamage(skillRangeConfig, index);
			}
		}

		void SetDamage(SkillRangeConfig skillRangeConfig, int index) {
			int damage = skillConfig.SkillDamageLst[index];
			if(attacker.entityType == EntityType.Player) {
				List<EntityMonster> monsterList = attacker.battleMgr.GetEntityMonsters();
				foreach(EntityMonster monster in monsterList) {
					if(InRange(attacker.Position, monster.Position, skillRangeConfig.Radius) &&
					   InAngle(attacker.Transform, monster.Position, skillRangeConfig.Angle)) {
						CalcDamage(attacker, monster, skillConfig, damage);
					}
				}
			}
			else if(attacker.entityType == EntityType.Monster) {
				EntityBase player = attacker.battleMgr.entityPlayer;
				if(InRange(attacker.Position, player.Position, skillRangeConfig.Radius) &&
				   InAngle(attacker.Transform, player.Position, skillRangeConfig.Angle)) {
					CalcDamage(attacker, player, skillConfig, damage);
				}
			}
		}
	}

	private void CalcDamage(EntityBase attacker, EntityBase target, SkillConfig skillConfig, int damage) {
		int sumDamage = damage;
		bool critical = false;
		BattleProps targetProps = target.Props;
		BattleProps attackProps = attacker.Props;
		switch(skillConfig.DmgType) {
			case(int)DamageType.AD:
				//计算闪避  
				int raDodge = Random.Range(1, 100);
				if(raDodge <= targetProps.Dodge) {
					target.SetDodge();
					return;
				}

				//计算人物属性加成
				sumDamage += attackProps.Ad;

				//计算暴击
				int raCritical = Random.Range(1, 100);
				critical = raCritical <= attackProps.Critical;
				if(critical) {
					float criticalRate = 1 + (Random.Range(1, 100) / 100.0f);
					sumDamage = (int)(sumDamage * criticalRate);
				}

				//计算穿甲
				int addef = (int)(1 - attackProps.Pierce / 100.0f) * targetProps.Addef;
				sumDamage -= addef;

				break;
			case(int)DamageType.Ap:
				//计算人物属性加成
				sumDamage += attackProps.Ap;
				//计算魔法抗性
				sumDamage -= targetProps.Apdef;
				break;
		}

		if(sumDamage < 0) {
			return;
		}

		if(critical) {
			target.SetCritical(sumDamage);
		}
		else {
			target.SetDamage(sumDamage);
		}

		if(target.Hp <= sumDamage) {
			target.Hp = 0;
			target.Die();
		}
		else {
			target.Hp -= sumDamage;
			if(target.GetBreakState() && target.entityState != EntityState.BaTi) {
				target.Hit();
			}
		}
	}

	private bool InRange(Vector3 from, Vector3 to, float range) {
		return Vector3.Distance(from, to) <= range;
	}

	private bool InAngle(Transform current, Vector3 target, float angle) {
		if(Math.Abs(angle - 360) <= float.MinValue) {
			return true;
		}

		Vector3 from = current.forward;
		Vector3 to = (target - current.position).normalized;
		return angle / 2 >= Vector3.Angle(from, to);
	}
}