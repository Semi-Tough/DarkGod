/****************************************************
	文件：TestPlayer.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 21:18   	
	功能：
*****************************************************/

using System;
using System.Collections;
using UnityEngine;
public class TestPlayer : MonoBehaviour
{
    public CharacterController Character;
    public Animator Animator;
    public GameObject Skill1Effect;

    private Transform cameraTrans;
    private Vector3 cameraOffset;
    private Vector2 characterDir;
    private float currentBlend;
    private float targetBlend;
    private bool isMove;

    private static readonly int Blend = Animator.StringToHash("Blend");
    private static readonly int Attack = Animator.StringToHash("Attack");

    private void Start()
    {
        if (Camera.main != null) cameraTrans = Camera.main.transform;
        cameraOffset = transform.position - cameraTrans.position;
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(h, v).normalized;
        CharacterDir = dir;


        if (isMove)
        {
            //设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCamera();
        }

        if (Math.Abs(currentBlend - targetBlend) > float.MinValue)
        {
            UpdateMixBlend();
        }
    }


    private Vector2 CharacterDir
    {
        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
                SetBlend(Constants.BlendIdle);
            }
            else
            {
                isMove = true;
                SetBlend(Constants.BlendMove);
            }

            characterDir = value;
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(characterDir, new Vector2(0, 1)) +
                      cameraTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    private void SetMove()
    {
        Character.Move(transform.forward * (Constants.PlayerMoveSpeed * Time.deltaTime));
    }

    private void SetCamera()
    {
        if (cameraTrans != null)
            cameraTrans.position = transform.position - cameraOffset;
    }

    private void SetBlend(float blend)
    {
        targetBlend = blend;
    }

    private void UpdateMixBlend() {
        float smoothValue = Constants.AccMove * Time.deltaTime;
        if (Mathf.Abs(currentBlend - targetBlend) <smoothValue)
            currentBlend = targetBlend;
        else if (currentBlend < targetBlend)
            currentBlend += smoothValue;
        else
            currentBlend -= smoothValue;

        Animator.SetFloat(Blend, currentBlend);
    }

    #region 点击事件

    public void ClickBtnSkill1()
    {
        Animator.SetInteger(Attack, 1);
        Skill1Effect.SetActive(true);
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.9f);
        Animator.SetInteger(Attack, -1);
        Skill1Effect.SetActive(false);
    }

    #endregion
}