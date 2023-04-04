/****************************************************
    文件：PlayerController
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月04日 星期五 12:59
    功能：角色表现实体控制器
*****************************************************/

using System;
using UnityEngine;
public class PlayerController : Controller {
    public GameObject Skill1Fx;
    public GameObject Skill2Fx;
    public GameObject Skill3Fx;

    public GameObject Attack1Fx;
    public GameObject Attack2Fx;
    public GameObject Attack3Fx;
    public GameObject Attack4Fx;
    public GameObject Attack5Fx;

    private Vector3 cameraOffset;


    public override void Init() {
        base.Init();
        if (Camera.main != null) cameraTrans = Camera.main.transform;
        cameraOffset = transform.position - cameraTrans.position;

        effectsDic.Add(Skill1Fx.name, Skill1Fx);
        effectsDic.Add(Skill2Fx.name, Skill2Fx);
        effectsDic.Add(Skill3Fx.name, Skill3Fx);
        effectsDic.Add(Attack1Fx.name, Attack1Fx);
        effectsDic.Add(Attack2Fx.name, Attack2Fx);
        effectsDic.Add(Attack3Fx.name, Attack3Fx);
        effectsDic.Add(Attack4Fx.name, Attack4Fx);
        effectsDic.Add(Attack5Fx.name, Attack5Fx);
    }

    private void SetDir() {
        float angle = Vector2.SignedAngle(moveDir, new Vector2(0, 1)) +
                      cameraTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }
    
    private void SetMove() {
        Character.Move(transform.forward * (Constants.PlayerMoveSpeed * Time.deltaTime));
    }

    public void SetCamera() {
        if (cameraTrans != null) cameraTrans.position = transform.position - cameraOffset;
    }

    private void SetSkillMove() {
        Character.Move(transform.forward * (skillMoveSpeed * Time.deltaTime));
    }

    private void Update() {
        if (isMove) {
            //设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCamera();
        }

        if (skillMove) {
            SetSkillMove();
            //相机跟随
            SetCamera();
        }

        if (Math.Abs(currentBlend - targetBlend) > float.MinValue) {
            UpdateMixBlend();
        }
    }

    private void UpdateMixBlend() {
        float smoothValue = Constants.AccMove * Time.deltaTime;
        if (Mathf.Abs(currentBlend - targetBlend) < smoothValue)
            currentBlend = targetBlend;
        else if (currentBlend < targetBlend)
            currentBlend += smoothValue;
        else
            currentBlend -= smoothValue;

        Animator.SetFloat(Blend, currentBlend);
    }

    public override void SetBlend(float blend) {
        targetBlend = blend;
    }

    public override void SetAttackFX(string effectName, uint delay) {
        if (!effectsDic.TryGetValue(effectName, out GameObject effect)) return;
        effect.SetActive(true);
        timerSvc.AddTask(delay,tid => {
            effect.SetActive(false);
        } );
    }
}