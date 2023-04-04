/****************************************************
    文件：MonsterController.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/10/30 19:57:22
    功能：怪物表现实体角色控制器
*****************************************************/

using System;
using UnityEngine;
public class MonsterController : Controller {
	public Transform HUDRoot;
	private void Update() {
		if(isMove) {
			SetDir();
			SetMove();
		}
		if(Math.Abs(currentBlend - targetBlend) > float.MinValue) {
			UpdateMixBlend();
		}
	}
	private void SetDir() {
		float angle = Vector2.SignedAngle(moveDir, new Vector2(0, 1));
		Vector3 eulerAngle = new Vector3(0, angle, 0);
		transform.localEulerAngles = eulerAngle;
	}
	private void SetMove() {
		Character.Move((transform.forward + Vector3.down) * (Constants.MonsterMoveSpeed * Time.deltaTime));
	}
	public override void SetBlend(float blend) {
		targetBlend = blend;
	}
	private void UpdateMixBlend() {
		float smoothValue = Constants.AccMove * Time.deltaTime;
		if(Mathf.Abs(currentBlend - targetBlend) < smoothValue)
			currentBlend = targetBlend;
		else if(currentBlend < targetBlend)
			currentBlend += smoothValue;
		else
			currentBlend -= smoothValue;

		Animator.SetFloat(Blend, currentBlend);
	}
}