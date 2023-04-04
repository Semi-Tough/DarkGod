/****************************************************
	文件：BattleManager.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 14:54   	
	功能：战场管理器
*****************************************************/

using cfg;
using PEProtocol;
using PETool.PELogger;
using System;
using System.Collections.Generic;
using UnityEngine;
public class BattleMgr : MonoBehaviour {
	public bool nextDoor;
	public bool isPause;
	private AudioSvc audioSvc;
	private Dictionary<string, EntityMonster> entityMonsterDic;
	public EntityPlayer entityPlayer;
	private MapConfig mapCfg;

	private MapMgr mapMgr;
	private ResSvc resSvc;

	private SkillMgr skillMgr;
	private StateMgr stateMgr;
	private TimerSvc timerSvc;
	private void Update() {
		if(entityPlayer != null && entityPlayer.curAniState != AniState.Die) {
			foreach(KeyValuePair<string, EntityMonster> monster in entityMonsterDic) {
				monster.Value.TickAILogic();
			}
			if(entityMonsterDic.Count == 0 && nextDoor) {
				bool hasNext = mapMgr.OpenNextDoor();
				nextDoor = false;
				if(!hasNext) {
					EndBattle(true, entityPlayer.Hp);
				}
			}
		}
	}

	public void Init(int mapId, Action cb = null) {
		resSvc = ResSvc.Instance;
		audioSvc = AudioSvc.Instance;
		timerSvc = TimerSvc.Instance;

		entityMonsterDic = new Dictionary<string, EntityMonster>();
		//初始化各管理器
		skillMgr = gameObject.AddComponent<SkillMgr>();
		skillMgr.Init();
		stateMgr = gameObject.AddComponent<StateMgr>();
		stateMgr.Init();

		//加载战场地图
		mapCfg = resSvc.GetMapConfig(mapId);
		resSvc.AsyncLoadScene(mapCfg.SceneName, () => {
			//初始化地图的数据
			GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
			mapMgr = map.GetComponent<MapMgr>();
			mapMgr.Init(this);

			//校正地图和相机的位置
			map.transform.localPosition = Vector3.zero;
			map.transform.localScale = Vector3.one;
			Camera mainCamera = Camera.main;
			if(mainCamera != null) {
				mainCamera.transform.position = resSvc.GetMapConfig(mapId).MainCamPos;
				mainCamera.transform.localEulerAngles = resSvc.GetMapConfig(mapId).MainCamRote;
			}

			LoadPlayer();
			// ActiveCurrentWaveMonster();

			audioSvc.PlayBgMusic(Constants.BgDungeon);
			cb?.Invoke();
		});

		PELogger.Log("战场管理器加载完成");
	}
	public void EndBattle(bool isWin, int restHp) {
		isPause = true;
		BattleSys.Instance.EndBattle(isWin, restHp);
		audioSvc.StopBgMusic();
	}
	private void LoadPlayer() {
		GameObject prefab = resSvc.LoadPrefab(PathDefine.BattlePlayerPrefab, true);

		if(prefab == null) return;
		GameObject player = Instantiate(prefab, mapCfg.PlayerBornPos,
			Quaternion.Euler(mapCfg.PlayerBornRote));
		player.transform.position = mapCfg.PlayerBornPos;
		player.transform.localEulerAngles = mapCfg.PlayerBornRote;
		player.transform.localScale = Vector3.one;

		if(Camera.main == null) return;
		Transform cameraTrans = Camera.main.transform;
		cameraTrans.position = mapCfg.MainCamPos;
		cameraTrans.localEulerAngles = mapCfg.MainCamRote;

		PlayerData playerData = GameRoot.Instance.PlayerData;
		BattleProps battleProps = new BattleProps(playerData.Hp, playerData.Ad, playerData.Ap, playerData.AdDef, playerData.ApDef, playerData.Dodge, playerData.Pierce, playerData.Critical);

		entityPlayer = new EntityPlayer{
			stateMgr = stateMgr,
			skillMgr = skillMgr,
			battleMgr = this
		};

		entityPlayer.SetBattleProps(battleProps);

		PlayerController playerController = player.GetComponent<PlayerController>();
		playerController.Init();
		entityPlayer.SetController(playerController);
		entityPlayer.Idle();
	}

	public void LoadMonsterByWaveId(int wave) {
		for(int i = 0; i < mapCfg.MonsterList.Count; i++) {
			MapMonster mapMonster = mapCfg.MonsterList[i];
			if(mapMonster.Wave != wave) continue;
			MonsterConfig monsterConfig = resSvc.GetMonsterConfig(mapMonster.MonsterId);

			string monsterName = $"{monsterConfig.Name}_{i.ToString()}";


			GameObject monster = Instantiate(resSvc.LoadPrefab(monsterConfig.ResPath));
			monster.name = monsterName;

			monster.transform.localPosition = mapMonster.BornPos;
			monster.transform.localEulerAngles = new Vector3(0, mapMonster.BornRotY, 0);
			monster.transform.localScale = Vector3.one;

			EntityMonster entityMonster = new EntityMonster{
				stateMgr = stateMgr,
				skillMgr = skillMgr,
				battleMgr = this,
				mapMonster = mapMonster,
				monsterConfig = resSvc.GetMonsterConfig(mapMonster.MonsterId),
				Name = monster.name
			};
			entityMonster.SetBattleProps(monsterConfig.Props);

			MonsterController monsterController = monster.GetComponent<MonsterController>();
			monsterController.Init();
			entityMonster.SetController(monsterController);
			monster.SetActive(false);

			entityMonsterDic.Add(monsterName, entityMonster);

			if(monsterConfig.Type == (int)MonsterType.Normal) {
				BattleSys.Instance.BattlePanel.AddMonsterHUDItem(monster.name, monsterController.HUDRoot, entityMonster.Props.Hp);
			}
			else if(monsterConfig.Type == (int)MonsterType.Boss) {
				BattleSys.Instance.BattlePanel.SetBossLifeBar();
			}
		}
	}

	public void ActiveCurrentWaveMonster() {
		foreach(KeyValuePair<string, EntityMonster> monster in entityMonsterDic) {
			timerSvc.AddTask(1500, tid => {
				monster.Value.SetActive(true);
				monster.Value.Born();
			});
		}
	}

	public void RemoveDeadMonster(string name) {
		if(entityMonsterDic.TryGetValue(name, out EntityMonster monster)) {
			BattleSys.Instance.BattlePanel.RemoveMonsterHUDItem(name);
			entityMonsterDic.Remove(name);
		}
	}

	public List<EntityMonster> GetEntityMonsters() {
		List<EntityMonster> monstersList = new List<EntityMonster>();
		foreach(KeyValuePair<string, EntityMonster> monster in entityMonsterDic) {
			monstersList.Add(monster.Value);
		}

		return monstersList;
	}

	#region 设置玩家移动和攻击
	public void SetPlayerMove(Vector2 dir) {
		if(entityPlayer.canControl == false) return;
		if(entityPlayer.curAniState is AniState.Idle or AniState.Move) {
			if(dir == Vector2.zero) {
				entityPlayer.Idle();
			}
			else {
				entityPlayer.Move();
				entityPlayer.SetMoveDir(dir);
			}
		}
	}

	public void SetPlayerAttack(int index) {
		switch(index) {
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

	private double lastAtkTime;
	private int atkIdIndex;
	private readonly int[] atkIdArr ={ 111, 112, 113, 114, 115 };

	private void NormalAttack() {
		double nowTime = timerSvc.GetNowTime();
		if(entityPlayer.curAniState == AniState.Idle) {
			if(nowTime - lastAtkTime < Constants.AtkTimeVal) {
				if(entityPlayer.canCombo) {
					atkIdIndex = atkIdIndex < atkIdArr.Length - 1 ? atkIdIndex += 1 : 0;
					entityPlayer.Attack(atkIdArr[atkIdIndex]);
					lastAtkTime = timerSvc.GetNowTime();
					entityPlayer.canCombo = false;
				}
			}
			else {
				atkIdIndex = 0;
				entityPlayer.Attack(atkIdArr[atkIdIndex]);
				lastAtkTime = timerSvc.GetNowTime();
				entityPlayer.canCombo = false;
			}
		}
	}

	private void Skill1() {
		entityPlayer.Attack(101);
	}

	private void Skill2() {
		entityPlayer.Attack(102);
	}

	private void Skill3() {
		entityPlayer.Attack(103);
	}

	public Vector2 GetMoveDir() {
		return BattleSys.Instance.GetMoveDir();
	}

	public bool CanRelSkill() {
		return entityPlayer.canRelSkill;
	}
	#endregion
}