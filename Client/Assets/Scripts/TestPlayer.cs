/****************************************************
	文件：TestPlayer.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 21:18   	
	功能：
*****************************************************/

using System;
using System.Collections;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public CharacterController character;
    public Animator animator;

    public GameObject skill1Effect;
    private Transform _cameraTrans;
    private Vector3 _cameraOffset;
    private Vector2 _characterDir;


    private float _currentBlend;
    private float _targetBlend;
    private bool _isMove;

    private static readonly int Blend = Animator.StringToHash("Blend");
    private static readonly int Action = Animator.StringToHash("Attack");

    private void Start()
    {
        if (Camera.main != null) _cameraTrans = Camera.main.transform;
        _cameraOffset = transform.position - _cameraTrans.position;
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(h, v).normalized;
        CharacterDir = dir;


        if (_isMove)
        {
            //设置方向
            SetDir();
            //产生移动
            SetMove();
            //相机跟随
            SetCamera();
        }

        if (Math.Abs(_currentBlend - _targetBlend) > float.MinValue)
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
                _isMove = false;
                SetBlend(Constants.BlendIdle);
            }
            else
            {
                _isMove = true;
                SetBlend(Constants.BlendMove);
            }

            _characterDir = value;
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(_characterDir, new Vector2(0, 1)) +
                      _cameraTrans.eulerAngles.y;
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    private void SetMove()
    {
        character.Move(transform.forward * (Constants.PlayerMoveSpeed * Time.deltaTime));
    }

    private void SetCamera()
    {
        if (_cameraTrans != null)
            _cameraTrans.position = transform.position - _cameraOffset;
    }

    private void SetBlend(float blend)
    {
        _targetBlend = blend;
    }

    private void UpdateMixBlend()
    {
        if (Mathf.Abs(_currentBlend - _targetBlend) < Constants.AcceleratedSpeed * Time.deltaTime)
            _currentBlend = _targetBlend;
        else if (_currentBlend < _targetBlend)
            _currentBlend += Constants.AcceleratedSpeed * Time.deltaTime;
        else
            _currentBlend -= Constants.AcceleratedSpeed * Time.deltaTime;

        animator.SetFloat(Blend, _currentBlend);
    }

    #region 点击事件

    public void ClickBtnSkill1()
    {
        animator.SetInteger(Action, 1);
        skill1Effect.SetActive(true);
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.9f);
        animator.SetInteger(Action, -1);
        skill1Effect.SetActive(false);
    }

    #endregion
}