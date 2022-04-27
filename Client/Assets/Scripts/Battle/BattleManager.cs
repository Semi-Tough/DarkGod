/****************************************************
	文件：BattleManager.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 14:54   	
	功能：战场管理器
*****************************************************/

using PEProtocol;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private ResourceService _resourceService;
    private AudioService _audioService;

    private MapManager _mapManager;
    private SkillManager _skillManager;
    private StateManager _stateManager;
    private EntityPlayer _entityPlayer;

    public void Init(int mapId)
    {
        _resourceService = ResourceService.instance;
        _audioService = AudioService.instance;
        //初始化各管理器
        _skillManager = gameObject.AddComponent<SkillManager>();
        _skillManager.Init();
        _stateManager = gameObject.AddComponent<StateManager>();
        _stateManager.Init();

        //加载战场地图
        MapConfig mapConfig = _resourceService.GetMapConfig(mapId);
        StartCoroutine(_resourceService.AsyncLoadScene(mapConfig.sceneName, () =>
        {
            //初始化地图的数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            _mapManager = map.GetComponent<MapManager>();
            _mapManager.Init();

            //校正地图和相机的位置
            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;
            if (Camera.main != null)
            {
                Camera.main.transform.position = _resourceService.GetMapConfig(mapId).mainCamPos;
                Camera.main.transform.localEulerAngles = _resourceService.GetMapConfig(mapId).mainCamRote;
            }

            LoadPlayer(mapConfig);
            _audioService.PlayBgMusic(Constants.BgDungeon);
        }));

        PeCommon.Log("战场管理器加载完成");
    }

    private void LoadPlayer(MapConfig mapConfig)
    {
        GameObject prefab = _resourceService.LoadPrefab(PathDefine.BattlePlayerPrefab, true);

        if (prefab == null) return;
        GameObject player = Instantiate(prefab, mapConfig.playerBornPos,
            Quaternion.Euler(mapConfig.playerBornRote));
        player.transform.position = mapConfig.playerBornPos;
        player.transform.localEulerAngles = mapConfig.playerBornRote;
        player.transform.localScale = Vector3.one;

        if (Camera.main == null) return;
        Transform cameraTrans = Camera.main.transform;
        cameraTrans.position = mapConfig.mainCamPos;
        cameraTrans.localEulerAngles = mapConfig.mainCamRote;

        _entityPlayer = new EntityPlayer
        {
            stateManager = _stateManager,
            skillManager = _skillManager,
            battleManager = this
        };
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Init();
        _entityPlayer.controller = playerController;
        _entityPlayer.Idle();
    }

    #region 设置玩家移动和攻击

    public void SetPlayerMove(Vector2 dir)
    {
        if (_entityPlayer.canControl == false) return;
        if (dir == Vector2.zero)
        {
            _entityPlayer.Idle();
        }
        else
        {
            _entityPlayer.Move();
            _entityPlayer.SetDirection(dir);
        }
    }

    public void SetPlayerAttack(int index)
    {
        switch (index)
        {
            case 0:
                NormalAttack();
                break;
            case 1:
                Skill1();
                break;
            case 2:
                Skill2();
                break;
            case 3:
                Skill3();
                break;
        }
    }

    private void NormalAttack()
    {
        PeCommon.Log("Normal SetPlayerAttack");
    }

    private void Skill1()
    {
        _entityPlayer.Attack(101);
    }

    private void Skill2()
    {
        PeCommon.Log("Skill2");
    }

    private void Skill3()
    {
        PeCommon.Log("Skill3");
    }

    public Vector2 GetInputDic()
    {
        return BattleSystem.Instance.GetInPutDic();
    }

    #endregion
}