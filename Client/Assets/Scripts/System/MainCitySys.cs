/****************************************************
    文件：MainCitySystem
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月02日 星期三 14:26
    功能：主城业务系统
*****************************************************/

using cfg;
using PEProtocol;
using PETool.PELogger;
using UnityEngine;
using UnityEngine.AI;
public class MainCitySys : SystemRoot {
	public static MainCitySys Instance { get; private set; }

	public MainCityPanel MainCityPanel;
	public InfoPanel InfoPanel;
	public GuidePanel GuidePanel;
	public StrengthenPanel StrengthenPanel;
	public ChatPanel ChatPanel;
	public BuyPanel BuyPanel;
	public TaskPanel TaskPanel;

	private NavMeshAgent meshAgent;
	private PlayerController playerController;

	public override void InitSystem() {
		base.InitSystem();
		Instance = this;
		PELogger.Log("主城系统加载完成");
	}

	public void LoadingMainCitySystem() {
		PELogger.Log("进入加载界面");
		MapConfig mapConfig = resSvc.GetMapConfig(Constants.MainCityID);
		resSvc.AsyncLoadScene(mapConfig.SceneName, () => {
			PELogger.Log("主城场景加载完成");
			MainCityPanel.SetPanelState();
			audioSvc.PlayBgMusic(Constants.BgMainCity);

			GameObject map = GameObject.FindWithTag("MapRoot");
			MainCityMap cityMap = map.GetComponent<MainCityMap>();
			npcPosition = cityMap.NpcPosition;

			//加载主角,设置人物展示相机
			LoadPlayer(mapConfig);
			if(showCamera != null) showCamera.gameObject.SetActive(false);
		});
	}

	private void LoadPlayer(MapConfig mapConfig) {
		GameObject prefab = resSvc.LoadPrefab(PathDefine.CityPlayerPrefab, true);

		if(prefab == null) return;
		GameObject player = Instantiate(prefab, mapConfig.PlayerBornPos,
			Quaternion.Euler(mapConfig.PlayerBornRote));
		player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		playerController = player.GetComponent<PlayerController>();
		meshAgent = player.GetComponent<NavMeshAgent>();

		if(Camera.main == null) return;
		Transform cameraTrans = Camera.main.transform;
		cameraTrans.position = mapConfig.MainCamPos;
		cameraTrans.localEulerAngles = mapConfig.MainCamRote;
		playerController.Init();
	}

	#region 设置玩家移动
	public void SetPlayerBlend(Vector2 dir) {
		StopNavTask();
		playerController.SetBlend(dir == Vector2.zero ? Constants.BlendIdle : Constants.BlendMove);
		playerController.SetMoveDir(dir);
	}
	#endregion

	#region 信息界面
	private Transform showCamera;

	public void OpenInfoPanel() {
		StopNavTask();

		if(showCamera == null) showCamera = GameObject.FindWithTag("ShowCamera").transform;

		showCamera.gameObject.SetActive(true);
		//设置相对位置
		Transform playerTrans = playerController.transform;
		showCamera.localPosition = playerTrans.localPosition +
		                           playerTrans.forward * 3.8f +
		                           new Vector3(0, 1.2f, 0);
		showCamera.localEulerAngles = new Vector3(0, 180 + playerTrans.localEulerAngles.y, 0);
		showCamera.localScale = Vector3.one;
		showCamera.gameObject.SetActive(true);
		InfoPanel.SetPanelState();
	}

	private float startRotateY;

	public void SetStartRotate() {
		startRotateY = playerController.transform.localEulerAngles.y;
	}

	public void SetPlayerRotate(float rotate) {
		playerController.transform.localEulerAngles =
			new Vector3(0, startRotateY - rotate * 0.4f, 0);
	}

	public void CloseInfoPanel() {
		showCamera.gameObject.SetActive(false);
		InfoPanel.SetPanelState(false);
	}
	#endregion

	#region 引导界面
	private Transform[] npcPosition;
	private GuideConfig currentTask;
	private bool isNavAgent;

	private void OpenGuidePanel() {
		GuidePanel.SetPanelState();
	}

	public GuideConfig GetGuideConfig() {
		return currentTask;
	}

	public void RunTask(GuideConfig guide) {
		if(guide == null) return;

		currentTask = guide;
		meshAgent.enabled = true;

		//解析任务数据
		if(currentTask.NpcId != -1) {
			Vector3 playerPos = playerController.transform.position;
			Vector3 npcPos = npcPosition[currentTask.NpcId].position;

			float distance = Vector2.Distance(new Vector2(playerPos.x, playerPos.z),
				new Vector2(npcPos.x, npcPos.z));

			if(distance < 0.5f) {
				StopNavTask();
				OpenGuidePanel();
			}
			else {
				isNavAgent = true;
				meshAgent.enabled = true;
				playerController.Character.enabled = false;
				meshAgent.speed = Constants.PlayerMoveSpeed;
				meshAgent.SetDestination(npcPosition[guide.NpcId].position);
				playerController.SetBlend(Constants.BlendMove);
			}
		}
		else {
			OpenGuidePanel();
		}
	}

	private void IsArriveTaskPosition() {
		Vector3 playerPos = playerController.transform.position;
		Vector3 npcPos = npcPosition[currentTask.NpcId].position;

		float distance = Vector2.Distance(new Vector2(playerPos.x, playerPos.z),
			new Vector2(npcPos.x, npcPos.z));

		if(distance < 0.5f) {
			StopNavTask();
			OpenGuidePanel();
		}
	}

	private void StopNavTask() {
		if(!isNavAgent) return;

		isNavAgent = false;
		meshAgent.isStopped = true;
		meshAgent.enabled = false;
		playerController.Character.enabled = true;
		playerController.SetBlend(Constants.BlendIdle);
	}

	public void ResponseGuide(NetMsg message) {
		ResponseGuide responseGuide = message.Body.ResponseGuide;
		GameRoot.AddTips(Constants.Color(
			"任务奖励: 金币+" + currentTask.Coin + "   经验值+" + currentTask.Exp, TextColor.Blue));
		switch(currentTask.Type) {
			case GuideType.Conversation:
				//与智者对话
				break;
			case GuideType.Dungeon:
				//进入副本
				EnterDungeonPanel();
				break;
			case GuideType.Strengthen:
				//进入强化界面
				OpenStrengthenPanel();
				break;
			case GuideType.BuyVit:
				//进入体力购买
				OpenBuyPanel(BuyType.Vit);
				break;
			case GuideType.BuyCoin:
				//进入金币铸造
				OpenBuyPanel(BuyType.Coin);
				break;
			case GuideType.Chat:
				//进入世界聊天
				OpenChatPanel();
				break;
		}

		if(message.Body.PushTask != null) {
			PushTask(message);
		}


		gameRoot.SetPlayerDataByGuide(responseGuide);
		MainCityPanel.RefreshUI();
	}

	private void Update() {
		if(!isNavAgent) return;
		playerController.SetCamera();
		IsArriveTaskPosition();
	}
	#endregion

	#region 强化界面
	public void OpenStrengthenPanel() {
		StopNavTask();
		StrengthenPanel.SetPanelState();
	}

	public void ResponseStrengthen(NetMsg message) {
		int oldFighting = Protocol.GetFightByProperty(gameRoot.PlayerData);
		gameRoot.SetPlayerDataByStrengthen(message.Body.ResponseStrengthen);
		int newFighting = Protocol.GetFightByProperty(gameRoot.PlayerData);
		GameRoot.AddTips(Constants.Color("战力提升 + " + (newFighting - oldFighting), TextColor.Red));

		if(message.Body.PushTask != null) {
			PushTask(message);
		}

		StrengthenPanel.UpdateStrengthenProperty();
		MainCityPanel.RefreshUI();
	}
	#endregion

	#region 聊天界面
	public void OpenChatPanel() {
		StopNavTask();
		ChatPanel.SetPanelState();
	}

	public void PushChat(NetMsg message) {
		ChatPanel.AddMessage(message.Body.PushChat.Name, message.Body.PushChat.Chat);
	}
	#endregion

	#region 交易界面
	public void OpenBuyPanel(BuyType type) {
		StopNavTask();
		BuyPanel.SetBuyType(type);
		BuyPanel.SetPanelState();
	}

	public void ResponseBuy(NetMsg message) {
		gameRoot.SetPlayerDataByBuy(message.Body.ResponseBuy);
		GameRoot.AddTips("购买成功...");

		if(message.Body.PushTask != null) {
			PushTask(message);
		}

		MainCityPanel.RefreshUI();
		BuyPanel.SetPanelState(false);
		BuyPanel.SetBtnInteractable();
	}
	#endregion

	#region 更新体力
	public void ResponsePower(NetMsg message) {
		GameRoot.AddTips("体力恢复: " + (message.Body.PushPower.Power - gameRoot.PlayerData.Power));
		gameRoot.SetPlayerPowerByPush(message.Body.PushPower);
		if(MainCityPanel.GetPanelState()) {
			MainCityPanel.RefreshUI();
		}
	}
	#endregion

	#region 任务界面
	public void OpenTaskPanel() {
		StopNavTask();
		TaskPanel.SetPanelState();
	}

	public void ResponseTask(NetMsg message) {
		ResponseTask responseTask = message.Body.ResponseTask;
		gameRoot.SetPlayerDataByTask(responseTask);
		TaskPanel.RefreshTask();
		MainCityPanel.RefreshUI();
		GameRoot.AddTips(
			Constants.Color($"任务奖励: 金币+ {responseTask.Coin.ToString()} " +
			                $"经验值+ {responseTask.Exp.ToString()}",
				TextColor.Blue));
	}

	public void PushTask(NetMsg message) {
		PushTask pushTask = message.Body.PushTask;
		gameRoot.SetTaskProgressByPush(pushTask);
		// if(TaskPanel.GetPanelState()) {
		// 	TaskPanel.RefreshTask();
		// }
	}
	#endregion

	#region 副本界面
	public void EnterDungeonPanel() {
		StopNavTask();
		DungeonSys.Instance.EnterDungeonPanel();
	}
	#endregion
}