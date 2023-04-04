/****************************************************
	文件：EntityBase.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 12:13   	
	功能：逻辑实体的基类
*****************************************************/

using cfg;
using System.Collections.Generic;
using UnityEngine;
public abstract class EntityBase {
	public AniState curAniState = AniState.None;

	public BattleMgr battleMgr;
	public SkillMgr skillMgr;
	public StateMgr stateMgr;

	public EntityType entityType = EntityType.None;
	public EntityState entityState = EntityState.None;

	public BattleProps Props { get; protected set; }
	public Controller controller;

	public bool canControl = true;
	public bool canRelSkill = true;
	public bool canCombo = true;

	public int skillEndCB = -1;
	public readonly List<int> skillMoveCBList = new List<int>();
	public readonly List<int> skillDamageCBList = new List<int>();

	protected int hp;
	public int Hp {
		get { return hp; }
		set {
			SetLifeBar(hp, value);
			hp = value;
		}
	}

	public string Name { get; set; }

	public virtual Vector2 GetMoveDir() {
		return Vector2.zero;
	}

	public void SetController(Controller controller) {
		this.controller = controller;
	}

	public void SetActive(bool active) {
		controller.gameObject.SetActive(active);
	}

	public uint GetAnimationClipTime(string name) {
		AnimationClip[] clips = controller.Animator.runtimeAnimatorController.animationClips;
		foreach(AnimationClip clip in clips) {
			if(clip.name.ToLower().Contains(name)) {
				return(uint)(clip.length * 1000 * 0.9f);
			}
		}

		//保护时间
		return 1000;
	}

	public Transform Transform { get { return controller.transform; } }
	public Vector3 Position { get { return controller.transform.position; } }

	#region ChangeState
	public void Born() {
		stateMgr.ChangeState(this, AniState.Born);
	}

	public void Idle() {
		stateMgr.ChangeState(this, AniState.Idle);
	}

	public void Move() {
		stateMgr.ChangeState(this, AniState.Move);
	}

	public void Attack(int skillId) {
		stateMgr.ChangeState(this, AniState.Attack, skillId);
	}

	public void Hit() {
		stateMgr.ChangeState(this, AniState.Hit);
	}

	public void Die() {
		stateMgr.ChangeState(this, AniState.Die);
	}
	#endregion

	#region SetUI
	public virtual void SetDodge() {
		if(controller) {
			BattleSys.Instance.BattlePanel.SetMonsterDodge(controller.name);
		}
	}

	public virtual void SetCritical(int damage) {
		if(controller) {
			BattleSys.Instance.BattlePanel.SetMonsterCritical(controller.name, damage);
		}
	}

	public virtual void SetDamage(int damage) {
		if(controller) {
			BattleSys.Instance.BattlePanel.SetMonsterDamage(controller.name, damage);
		}
	}

	public virtual void SetLifeBar(int oldHp, int newHp) {
		if(controller) {
			BattleSys.Instance.BattlePanel.SetMonsterLifeBar(controller.name, oldHp, newHp);
		}
	}
	#endregion

	public virtual void TickAILogic() {
	}
	
	public virtual void SetBattleProps(BattleProps props) {
		Props = props;
		hp = props.Hp;
	}

	public virtual void SetMoveDir(Vector2 dir) {
		if(controller != null) {
			controller.SetMoveDir(dir);
		}
	}

	public virtual void SetAttackDir(Vector2 dir, bool offset = false) {
		if(controller != null) {
			controller.SetAttackDir(dir, offset);
		}
	}

	public virtual void SetAttack(int skillId) {
		skillMgr.SetAttack(this, skillId);
	}

	public virtual void SetBlend(float blend) {
		if(controller != null) {
			controller.SetBlend(blend);
		}
	}

	public virtual void SetAniAction(int action) {
		if(controller != null) {
			controller.SetAction(action);
		}
	}

	public virtual void SetAttackFX(string effectName, uint delay) {
		if(controller != null) {
			controller.SetAttackFX(effectName, delay);
		}
	}

	public virtual void SetSkillMove(bool move, float speed = 0f) {
		if(controller != null) {
			controller.SetSkillMove(move, speed);
		}
	}

	public virtual bool GetBreakState() {
		return true;
	}
	public void RemoveSkillMove(int tid) {
		int index = -1;
		for(int i = 0; i < skillMoveCBList.Count; i++) {
			if(skillMoveCBList[i] == tid) {
				index = i;
			}
		}
		if(index != -1) {
			skillMoveCBList.RemoveAt(index);
		}
	}
	public void RemoveSkillDamage(int tid) {
		int index = -1;
		for(int i = 0; i < skillDamageCBList.Count; i++) {
			if(skillDamageCBList[i] == tid) {
				index = i;
			}
		}
		if(index != -1) {
			skillDamageCBList.RemoveAt(index);
		}
	}
	public void RemoveSkill() {
		List<int> moveList = skillMoveCBList;
		for(int i = 0; i < moveList.Count; i++) {
			TimerSvc.Instance.RemoveTask(moveList[i]);
		}

		List<int> damageList = skillDamageCBList;
		for(int i = 0; i < damageList.Count; i++) {
			TimerSvc.Instance.RemoveTask(damageList[i]);
		}

		skillMoveCBList.Clear();
		skillDamageCBList.Clear();
		SetSkillMove(false);

		if(skillEndCB != -1) {
			TimerSvc.Instance.RemoveTask(skillEndCB);
			skillEndCB = -1;
		}
	}
	
	public AudioSource GetAudio() {
		return controller.GetComponent<AudioSource>();
	}
}