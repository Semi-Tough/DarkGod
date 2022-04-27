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

    public PlayerData PlayerData { get; private set; }

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
        PlayerData = date.PlayerData;
    }

    public void SetPlayerDataByName(string playerName)
    {
        PlayerData.Name = playerName;
    }

    public void SetPlayerDataByGuide(ResponseGuide responseGuide)
    {
        PlayerData.Coin = responseGuide.Coin;
        PlayerData.Level = responseGuide.Level;
        PlayerData.Exp = responseGuide.Exp;
        PlayerData.GuideId = responseGuide.GuideId;
    }

    public void SetPlayerDataByStrengthen(ResponseStrengthen responseStrengthen)
    {
        PlayerData.Coin = responseStrengthen.Coin;
        PlayerData.Crystal = responseStrengthen.Crystal;
        PlayerData.Hp = responseStrengthen.Hp;
        PlayerData.Ad = responseStrengthen.Ad;
        PlayerData.Ap = responseStrengthen.Ap;
        PlayerData.AdDef = responseStrengthen.AdDef;
        PlayerData.ApDef = responseStrengthen.ApDef;
        PlayerData.StrengthenArr = responseStrengthen.StrengthenArr;
    }

    public void SetPlayerDataByTimePush(PushPower pushPower)
    {
        PlayerData.Power = pushPower.Power;
    }

    public void SetPlayerDataByBuy(ResponseBuy responseBuy)
    {
        PlayerData.Coin = responseBuy.Coin;
        PlayerData.Diamond = responseBuy.Diamond;
        PlayerData.Power = responseBuy.Power;
    }

    public void SetPlayerDataByTask(ResponseTask responseTask)
    {
        PlayerData.Coin = responseTask.Coin;
        PlayerData.Exp = responseTask.Exp;
        PlayerData.Level = responseTask.Level;
        PlayerData.TaskArr = responseTask.TaskArr;
    }

    public void SetPlayerDataByTaskPush(PushTask pushTask)
    {
        PlayerData.TaskArr = pushTask.TaskArr;
    }

    public void SetPlayerDataByDungeonPush(ResponseDungeon responseDungeon)
    {
        PlayerData.Power = responseDungeon.Power;
    }

    #endregion
}