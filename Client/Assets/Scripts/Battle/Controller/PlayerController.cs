/****************************************************
    文件：PlayerController
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月04日 星期五 12:59
    功能：角色表现实体控制器
*****************************************************/

using System;
using UnityEngine;

public class PlayerController : Controller
{
    public CharacterController character;
    public GameObject skill1Effect;
    private Transform _cameraTrans;
    private Vector3 _cameraOffset;


    public override void Init()
    {
        base.Init();
        if (Camera.main != null) _cameraTrans = Camera.main.transform;
        _cameraOffset = transform.position - _cameraTrans.position;

        if (skill1Effect != null)
        {
            effectsDic.Add(skill1Effect.name, skill1Effect);
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(characterDir, new Vector2(0, 1)) +
                      _cameraTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    private void SetMove()
    {
        character.Move(transform.forward * (Constants.PlayerMoveSpeed * Time.deltaTime));
    }

    private void SetSkillMove()
    {
        character.Move(transform.forward * (skillMoveSpeed * Time.deltaTime));
    }

    public void SetCamera()
    {
        if (_cameraTrans != null)
            _cameraTrans.position = transform.position - _cameraOffset;
    }

    private void Update()
    {
        if (isMove)
        {
            //设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCamera();
        }

        if (skillMove)
        {
            SetSkillMove();
            //相机跟随
            SetCamera();
        }

        if (Math.Abs(currentBlend - targetBlend) > float.MinValue)
        {
            UpdateMixBlend();
        }

    }

    public override void SetBlend(float blend)
    {
        targetBlend = blend;
    }

    public override void SetEffectState(string effectName, float delay)
    {
        if (!effectsDic.TryGetValue(effectName, out GameObject effect)) return;
        effect.SetActive(true);
        timerService.AddTimeTask(() => { effect.SetActive(false); }, delay, 1);
    }
}