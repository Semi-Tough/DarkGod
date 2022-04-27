/****************************************************
    文件：MainCitySystem
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月02日 星期三 14:26
    功能：主城业务系统
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.AI;

public class MainCitySystem : SystemRoot
{
    public static MainCitySystem Instance { get; private set; }

    public MainCityWindow mainCityWindow;
    public InfoWindow infoWindow;
    public GuideWindow guideWindow;
    public StrengthenWindow strengthenWindow;
    public ChatWindow chatWindow;
    public BuyWindow buyWindow;
    public TaskWindow taskWindow;

    private NavMeshAgent _meshAgent;
    private PlayerController _playerController;

    public override void InitSystem()
    {
        base.InitSystem();
        Instance = this;
        PeCommon.Log("主城系统加载完成");
    }

    public void LoadingMainCitySystem()
    {
        PeCommon.Log("进入加载界面");
        MapConfig mapConfig = resourceService.GetMapConfig(Constants.MainCityId);
        StartCoroutine(resourceService.AsyncLoadScene(mapConfig.sceneName, () =>
        {
            PeCommon.Log("主城场景加载完成");
            mainCityWindow.SetWindowState();
            audioService.PlayBgMusic(Constants.BgMainCity);

            GameObject map = GameObject.FindWithTag("MapRoot");
            MainCityMap cityMap = map.GetComponent<MainCityMap>();
            _npcPosition = cityMap.npcPosition;

            //加载主角,设置人物展示相机
            LoadPlayer(mapConfig);
            if (_showCamera != null) _showCamera.gameObject.SetActive(false);
        }));
    }

    private void LoadPlayer(MapConfig mapConfig)
    {
        GameObject prefab = resourceService.LoadPrefab(PathDefine.CityPlayerPrefab, true);

        if (prefab == null) return;
        GameObject player = Instantiate(prefab, mapConfig.playerBornPos,
            Quaternion.Euler(mapConfig.playerBornRote));
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        _playerController = player.GetComponent<PlayerController>();
        _meshAgent = player.GetComponent<NavMeshAgent>();

        if (Camera.main == null) return;
        Transform cameraTrans = Camera.main.transform;
        cameraTrans.position = mapConfig.mainCamPos;
        cameraTrans.localEulerAngles = mapConfig.mainCamRote;
        _playerController.Init();
    }

    #region 设置玩家移动

    private float _startRotateY;

    public void SetStartRotate()
    {
        _startRotateY = _playerController.transform.localEulerAngles.y;
    }

    public void SetPlayerRotate(float rotate)
    {
        _playerController.transform.localEulerAngles =
            new Vector3(0, _startRotateY - rotate * 0.4f, 0);
    }

    public void SetPlayerBlend(Vector2 dir)
    {
        StopNavTask();
        _playerController.CharacterDir = dir;
    }

    #endregion

    #region 信息界面

    private Transform _showCamera;

    public void OpenInfoWindow()
    {
        StopNavTask();

        if (_showCamera == null) _showCamera = GameObject.FindWithTag("ShowCamera").transform;

        _showCamera.gameObject.SetActive(true);
        //设置相对位置
        Transform playerTrans = _playerController.transform;
        _showCamera.localPosition = playerTrans.localPosition + playerTrans.forward * 3.8f +
                                    new Vector3(0, 1.2f, 0);
        _showCamera.localEulerAngles = new Vector3(0, 180 + playerTrans.localEulerAngles.y, 0);
        _showCamera.localScale = Vector3.one;
        _showCamera.gameObject.SetActive(true);
        infoWindow.SetWindowState();
    }

    public void CloseInfoWindow()
    {
        _showCamera.gameObject.SetActive(false);
        infoWindow.SetWindowState(false);
    }

    #endregion

    #region 引导界面

    private Transform[] _npcPosition;
    private GuideConfig _currentTask;
    private bool _isNavAgent;

    private void OpenGuideWindow()
    {
        guideWindow.SetWindowState();
    }

    public GuideConfig GetGuideConfig()
    {
        return _currentTask;
    }

    public void RunTask(GuideConfig guide)
    {
        if (guide == null) return;

        _currentTask = guide;
        _meshAgent.enabled = true;

        //解析任务数据
        if (_currentTask.npcId != -1)
        {
            Vector3 playerPos = _playerController.transform.position;
            Vector3 npcPos = _npcPosition[_currentTask.npcId].position;

            float distance = Vector2.Distance(new Vector2(playerPos.x, playerPos.z),
                new Vector2(npcPos.x, npcPos.z));

            if (distance < 0.5f)
            {
                StopNavTask();
                OpenGuideWindow();
            }
            else
            {
                _isNavAgent = true;
                _meshAgent.enabled = true;
                _playerController.character.enabled = false;
                _meshAgent.speed = Constants.PlayerMoveSpeed;
                _meshAgent.SetDestination(_npcPosition[guide.npcId].position);
                _playerController.SetBlend(Constants.BlendMove);
            }
        }
        else
        {
            OpenGuideWindow();
        }
    }

    private void IsArriveTaskPosition()
    {
        Vector3 playerPos = _playerController.transform.position;
        Vector3 npcPos = _npcPosition[_currentTask.npcId].position;

        float distance = Vector2.Distance(new Vector2(playerPos.x, playerPos.z),
            new Vector2(npcPos.x, npcPos.z));

        if (distance < 0.5f)
        {
            StopNavTask();
            OpenGuideWindow();
        }
    }

    private void StopNavTask()
    {
        if (!_isNavAgent) return;

        _isNavAgent = false;
        _meshAgent.isStopped = true;
        _meshAgent.enabled = false;
        _playerController.character.enabled = true;
        _playerController.SetBlend(Constants.BlendIdle);
    }

    public void ResponseGuide(GameMessage message)
    {
        ResponseGuide responseGuide = message.ResponseGuide;
        GameRoot.AddTips(Constants.Color(
            "任务奖励: 金币+" + _currentTask.coin + "   经验值+" + _currentTask.exp, TextColor.Blue));
        switch (_currentTask.actionId)
        {
            case 0:
                //与智者对话
                break;
            case 1:
                //进入副本
                EnterDungeonWindow();
                break;
            case 2:
                //进入强化界面
                OpenStrengthenWindow();
                break;
            case 3:
                //进入体力购买
                OpenBuyWindow(BuyType.Vit);
                break;
            case 4:
                //进入金币铸造
                OpenBuyWindow(BuyType.Coin);
                break;
            case 5:
                //进入世界聊天
                OpenChatWindow();
                break;
        }

        if (message.PushTask != null)
        {
            PushTask(message);
        }


        gameRoot.SetPlayerDataByGuide(responseGuide);
        mainCityWindow.RefreshUI();
    }

    private void Update()
    {
        if (!_isNavAgent) return;
        _playerController.SetCamera();
        IsArriveTaskPosition();
    }

    #endregion

    #region 强化界面

    public void OpenStrengthenWindow()
    {
        StopNavTask();
        strengthenWindow.SetWindowState();
    }

    public void ResponseStrengthen(GameMessage message)
    {
        int oldFighting = PeCommon.GetFightByProperty(gameRoot.PlayerData);
        gameRoot.SetPlayerDataByStrengthen(message.ResponseStrengthen);
        int newFighting = PeCommon.GetFightByProperty(gameRoot.PlayerData);
        GameRoot.AddTips(Constants.Color("战力提升 + " + (newFighting - oldFighting), TextColor.Red));

        if (message.PushTask != null)
        {
            PushTask(message);
        }

        strengthenWindow.UpdateStrengthenProperty();
        mainCityWindow.RefreshUI();
    }

    #endregion

    #region 聊天界面

    public void OpenChatWindow()
    {
        StopNavTask();
        chatWindow.SetWindowState();
    }

    public void PushChat(GameMessage message)
    {
        chatWindow.AddMessage(message.PushChat.Name, message.PushChat.Chat);
    }

    #endregion

    #region 交易界面

    public void OpenBuyWindow(BuyType type)
    {
        StopNavTask();
        buyWindow.SetBuyType(type);
        buyWindow.SetWindowState();
    }

    public void ResponseBuy(GameMessage message)
    {
        gameRoot.SetPlayerDataByBuy(message.ResponseBuy);
        GameRoot.AddTips("购买成功...");

        if (message.PushTask != null)
        {
            PushTask(message);
        }

        mainCityWindow.RefreshUI();
        buyWindow.SetWindowState(false);
        buyWindow.SetBtnInteractable();
    }

    #endregion

    #region 更新体力

    public void ResponsePower(GameMessage message)
    {
        GameRoot.AddTips("体力恢复: " + (message.PushPower.Power - gameRoot.PlayerData.Power));
        gameRoot.SetPlayerDataByTimePush(message.PushPower);
        if (mainCityWindow.GetWindowState())
        {
            mainCityWindow.RefreshUI();
        }
    }

    #endregion

    #region 任务界面

    public void OpenTaskWindow()
    {
        StopNavTask();
        taskWindow.SetWindowState();
    }

    public void ResponseTask(GameMessage message)
    {
        ResponseTask responseTask = message.ResponseTask;
        gameRoot.SetPlayerDataByTask(responseTask);
        taskWindow.RefreshUI();
        mainCityWindow.RefreshUI();
    }

    public void PushTask(GameMessage message)
    {
        PushTask pushTask = message.PushTask;
        gameRoot.SetPlayerDataByTaskPush(pushTask);
        if (taskWindow.GetWindowState())
        {
            taskWindow.RefreshUI();
        }
    }

    #endregion

    #region 副本界面

    public void EnterDungeonWindow()
    {
        StopNavTask();
        DungeonSystem.Instance.EnterDungeonWindow();
    }

    #endregion
}