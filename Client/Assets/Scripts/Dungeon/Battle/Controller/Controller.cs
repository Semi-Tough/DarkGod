/****************************************************
	文件：Controller.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 13:52   	
	功能：表现实体控制器基类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
public abstract class Controller : MonoBehaviour {
	public CharacterController Character;
	public Animator Animator;

	protected TimerSvc timerSvc;
	protected static readonly int Blend = Animator.StringToHash("Blend");
	protected static readonly int Action = Animator.StringToHash("Action");
	protected readonly Dictionary<string, GameObject> effectsDic = new();

	protected float currentBlend;
	protected float targetBlend;
	protected float skillMoveSpeed;

	public bool isMove;
	protected bool skillMove;
	protected Transform cameraTrans;

	protected Vector2 moveDir;
	public virtual void Init() {
		timerSvc = TimerSvc.Instance;
		Character.enabled = true;
	}
	public void SetMoveDir(Vector2 dir) {
		isMove = dir != Vector2.zero;
		moveDir = dir;
	}

	public void SetAttackDir(Vector2 dir, bool offset = false) {
		if(dir == Vector2.zero) {
			dir = moveDir;
		}
		float angle;
		if(offset && cameraTrans != null) {
			angle = Vector2.SignedAngle(dir, new Vector2(0, 1)) +
			        cameraTrans.eulerAngles.y;
		}
		else {
			angle = Vector2.SignedAngle(dir, new Vector2(0, 1));
		}
		Vector3 eulerAngle = new Vector3(0, angle, 0);
		transform.localEulerAngles = eulerAngle;
	}

	public virtual void SetBlend(float blend) {
		Animator.SetFloat(Blend, blend);
	}

	public virtual void SetAction(int action) {
		Animator.SetInteger(Action, action);
	}

	public virtual void SetAttackFX(string effectName, uint delay) {
	}

	public void SetSkillMove(bool move, float skillSpeed = 0) {
		skillMove = move;
		skillMoveSpeed = skillSpeed;
	}
}