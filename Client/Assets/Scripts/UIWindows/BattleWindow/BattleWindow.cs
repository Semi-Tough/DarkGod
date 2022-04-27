/****************************************************
	文件：BattleWindow.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 19:37   	
	功能：战斗控制界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : WindowRoot
{
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;

    public Text txtLevel;
    public Text txtName;
    public Text txtExpProgress;

    public GridLayoutGroup layoutGroup;

    private BattleSystem _battleSystem;

    private float _pointDistance;
    private Vector2 _startPosition;
    public Vector2 currentDir;

    protected override void InitWindow()
    {
        _battleSystem = BattleSystem.Instance;
        base.InitWindow();
        SetActive(imgDirPoint, false);
        RegisterTouchEvents();
        RefreshUI();
        _pointDistance = 1.0f * Screen.height / Constants.ScreenStandardHeight *
                         Constants.RockerOperateDistance;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResourceService.instance.ResetSkillConfigFiles();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            NormalAttack();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Skill1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Skill2();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Skill3();
        }
#endif
    }

    private void RefreshUI()
    {
        SetText(txtName, playerData.Name);
        SetText(txtLevel, playerData.Level);

        #region 进度条UI设置

        int expPrgValue = (int) (playerData.Exp * 1.0f /
            PeCommon.GetExpUpValueByLevel(playerData.Level) * 100);

        txtExpProgress.text =
            playerData.Exp + "/" + PeCommon.GetExpUpValueByLevel(playerData.Level);

        int index = expPrgValue / 10;
        for (int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            Image image = layoutGroup.transform.GetChild(i).GetComponent<Image>();
            if (index > i)
                image.fillAmount = 1;
            else if (index == i)
                image.fillAmount = expPrgValue % 10 * 1.0f / 10;
            else
                image.fillAmount = 0;
        }

        float heightRatio = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float screenWeight = Screen.width * heightRatio;
        float cellSizeX = (screenWeight - 172) / 10;
        layoutGroup.cellSize = new Vector2(cellSizeX, 8);

        #endregion
    }

    private void RegisterTouchEvents()
    {
        OnClickDown(imgTouch.gameObject, eventDate =>
        {
            imgDirBg.transform.position = eventDate.position;
            _startPosition = eventDate.position;
            SetActive(imgDirPoint);
        });

        OnDrag(imgTouch.gameObject, eventDate =>
        {
            Vector2 dir = eventDate.position - _startPosition;

            if (dir.magnitude > _pointDistance)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, _pointDistance);
                imgDirPoint.transform.position = _startPosition + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = eventDate.position;
            }

            currentDir = dir.normalized;
            _battleSystem.SetPlayerMove(currentDir);
        });

        OnClickUp(imgTouch.gameObject, eventData =>
        {
            imgDirBg.rectTransform.localPosition = Vector3.zero;
            imgDirPoint.rectTransform.localPosition = Vector3.zero;
            SetActive(imgDirPoint, false);
            currentDir = Vector2.zero;
            _battleSystem.SetPlayerMove(currentDir);
        });
    }

    #region 点击事件

    public void NormalAttack()
    {
        _battleSystem.SetPlayerAttack(0);
    }

    public void Skill1()
    {
        _battleSystem.SetPlayerAttack(1);
    }

    public void Skill2()
    {
        _battleSystem.SetPlayerAttack(2);
    }

    public void Skill3()
    {
        _battleSystem.SetPlayerAttack(3);
    }

    #endregion
}