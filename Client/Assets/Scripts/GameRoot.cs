/****************************************************
    文件：GameRoot.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/22 12:45:29
    功能：游戏启动入口
*****************************************************/

using PEProtocol;
using PETool.PELogger;
using UnityEngine;
public class GameRoot : MonoBehaviour {
	public static GameRoot Instance { private set; get; }
	public LoadingPanel LoadingPanel;
	public TipsPanel TipsPanel;

	public PlayerData PlayerData { get; private set; }

	private void Start() {
		Instance = this;
		DontDestroyOnLoad(this);
		ClearUiRoot();
		Init();
	}

	private void Init() {
		PELogger.InitSetting(new LogConfig{
			enableLog = true,
			enableTime = true,
			enableSave = true,
			enableCover = true,
			enableTrace = true,
			enableThreadId = true,
			loggerType = LoggerType.Unity,
		});

		//服务模块初始化
		NetSvc netSvc = GetComponent<NetSvc>();
		netSvc.InitService();
		ResSvc resSvc = GetComponent<ResSvc>();
		resSvc.InitService();
		AudioSvc audioSvc = GetComponent<AudioSvc>();
		audioSvc.InitService();
		TimerSvc timerSvc = GetComponent<TimerSvc>();
		timerSvc.InitService();

		//业务系统初始化
		LoginSys loginSys = GetComponent<LoginSys>();
		loginSys.InitSystem();
		MainCitySys mainCitySys = GetComponent<MainCitySys>();
		mainCitySys.InitSystem();
		DungeonSys dungeonSys = GetComponent<DungeonSys>();
		dungeonSys.InitSystem();
		BattleSys battleSys = GetComponent<BattleSys>();
		battleSys.InitSystem();

		//进入登陆场景并加载相应UI
		loginSys.LoadingLoginPanel();
	}

	private void ClearUiRoot() {
		Transform canvas = transform.Find("Canvas");
		for(int i = 0; i < canvas.childCount; i++) canvas.GetChild(i).gameObject.SetActive(false);

		TipsPanel.SetPanelState();
		TipsPanel.transform.GetChild(0).gameObject.SetActive(false);
	}

	public static void AddTips(string str) {
		Instance.TipsPanel.AddTips(str);
	}

	#region 设置玩家数据
	public void SetPlayerData(ResponseLogin date) {
		PlayerData = date.PlayerData;
	}

	public void SetPlayerName(string playerName) {
		PlayerData.Name = playerName;
	}

	public void SetPlayerDataByGuide(ResponseGuide responseGuide) {
		PlayerData.Coin = responseGuide.Coin;
		PlayerData.Level = responseGuide.Level;
		PlayerData.Exp = responseGuide.Exp;
		PlayerData.GuideId = responseGuide.GuideId;
	}

	public void SetPlayerDataByStrengthen(ResponseStrengthen responseStrengthen) {
		PlayerData.Coin = responseStrengthen.Coin;
		PlayerData.Crystal = responseStrengthen.Crystal;
		PlayerData.Hp = responseStrengthen.Hp;
		PlayerData.Ad = responseStrengthen.Ad;
		PlayerData.Ap = responseStrengthen.Ap;
		PlayerData.AdDef = responseStrengthen.AdDef;
		PlayerData.ApDef = responseStrengthen.ApDef;
		PlayerData.StrengthenDatas = responseStrengthen.StrengthenArrs;
	}

	public void SetPlayerPowerByPush(PushPower pushPower) {
		PlayerData.Power = pushPower.Power;
	}

	public void SetPlayerDataByBuy(ResponseBuy responseBuy) {
		PlayerData.Coin = responseBuy.Coin;
		PlayerData.Diamond = responseBuy.Diamond;
		PlayerData.Power = responseBuy.Power;
	}

	public void SetPlayerDataByTask(ResponseTask responseTask) {
		PlayerData.Coin = responseTask.Coin;
		PlayerData.Exp = responseTask.Exp;
		PlayerData.Level = responseTask.Level;
		PlayerData.TaskDatas=responseTask.TaskLists;
	}

	public void SetTaskProgressByPush(PushTask pushTask) {
		PlayerData.TaskDatas=pushTask.TaskLists;
	}

	public void SetPlayerPowerByDungeon(ResponseDungeon responseDungeon) {
		PlayerData.Power = responseDungeon.Power;
	}

	public void SetPlayerDataByEndDungeon(ResponseEndDungeon responseEndDungeon) {
		PlayerData.Coin = responseEndDungeon.Coin;
		PlayerData.Exp = responseEndDungeon.Exp;
		PlayerData.Level = responseEndDungeon.Level;
		PlayerData.Crystal = responseEndDungeon.Crystal;
		PlayerData.DungeonId = responseEndDungeon.Dungeon;
	}
	#endregion
}