/****************************************************
    文件：GameRoot.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/22 12:45:29
    功能：游戏启动入口
*****************************************************/

using PEProtocol;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public static GameRoot instance;
    public LoadingWindow loadingWindow;
    public TipsWindow tipsWindow;

    public PlayerData playerData { get; private set; }

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(this);
        ClearUiRoot();
        Init();
    }

    private void Init()
    {
        //服务模块初始化
        NetService netService = GetComponent<NetService>();
        netService.InitService();
        ResourceService resourceService = GetComponent<ResourceService>();
        resourceService.InitService();
        AudioService audioService = GetComponent<AudioService>();
        audioService.InitService();
        TimerService timerService = GetComponent<TimerService>();
        timerService.InitService();

        //业务系统初始化
        LoginSystem loginSystem = GetComponent<LoginSystem>();
        loginSystem.InitSystem();
        MainCitySystem mainCitySystem = GetComponent<MainCitySystem>();
        mainCitySystem.InitSystem();
        DungeonSystem dungeonSystem = GetComponent<DungeonSystem>();
        dungeonSystem.InitSystem();
        BattleSystem battleSystem = GetComponent<BattleSystem>();
        battleSystem.InitSystem();

        //进入登陆场景并加载相应UI
        loginSystem.LoadingLoginWindow();
    }

    private void ClearUiRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++) canvas.GetChild(i).gameObject.SetActive(false);

        tipsWindow.SetWindowState();
        tipsWindow.transform.GetChild(0).gameObject.SetActive(false);
    }

    public static void AddTips(string str)
    {
        instance.tipsWindow.AddTips(str);
    }

    #region 设置玩家数据

    public void SetPlayerDataByLogin(ResponseLogin date)
    {
        playerData = date.PlayerData;
    }

    public void SetPlayerDataByName(string playerName)
    {
        playerData.Name = playerName;
    }

    public void SetPlayerDataByGuide(ResponseGuide responseGuide)
    {
        playerData.Coin = responseGuide.Coin;
        playerData.Level = responseGuide.Level;
        playerData.Exp = responseGuide.Exp;
        playerData.GuideId = responseGuide.GuideId;
    }

    public void SetPlayerDataByStrengthen(ResponseStrengthen responseStrengthen)
    {
        playerData.Coin = responseStrengthen.Coin;
        playerData.Crystal = responseStrengthen.Crystal;
        playerData.Hp = responseStrengthen.Hp;
        playerData.Ad = responseStrengthen.Ad;
        playerData.Ap = responseStrengthen.Ap;
        playerData.AdDef = responseStrengthen.AdDef;
        playerData.ApDef = responseStrengthen.ApDef;
        playerData.StrengthenArr = responseStrengthen.StrengthenArr;
    }

    public void SetPlayerDataByTimePush(PushPower pushPower)
    {
        playerData.Power = pushPower.Power;
    }

    public void SetPlayerDataByBuy(ResponseBuy responseBuy)
    {
        playerData.Coin = responseBuy.Coin;
        playerData.Diamond = responseBuy.Diamond;
        playerData.Power = responseBuy.Power;
    }

    public void SetPlayerDataByTask(ResponseTask responseTask)
    {
        playerData.Coin = responseTask.Coin;
        playerData.Exp = responseTask.Exp;
        playerData.Level = responseTask.Level;
        playerData.TaskArr = responseTask.TaskArr;
    }

    public void SetPlayerDataByTaskPush(PushTask pushTask)
    {
        playerData.TaskArr = pushTask.TaskArr;
    }

    public void SetPlayerDataByDungeonPush(ResponseDungeon responseDungeon)
    {
        playerData.Power = responseDungeon.Power;
    }

    #endregion
}