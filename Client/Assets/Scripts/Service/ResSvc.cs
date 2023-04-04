/****************************************************
    文件：ResSvc.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/22 12:44:44
    功能：资源服务
*****************************************************/

using cfg;
using PETool.PELogger;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ResSvc : MonoBehaviour {
	public static ResSvc Instance { get; private set; }

	private Tables tables;
	public Dictionary<int, GuideConfig> guideDic = new();
	public Dictionary<int, MapConfig> mapDic = new();
	public Dictionary<int, MonsterConfig> monsterDic = new();
	public Dictionary<int, NameConfig> nameDic = new();
	public Dictionary<int, SkillConfig> skillDic = new();
	public Dictionary<int, SkillMoveConfig> skillMoveDic = new();
	public Dictionary<int, SkillRangeConfig> skillRangeDic = new();
	public Dictionary<int, StrengthenConfig> strengthenDic = new();
	public Dictionary<int, TaskRewardConfig> taskRewardDic = new();

	public void InitService() {
		Instance = this;
		tables = new Tables(JsonLoader);
		guideDic = tables.TbGuide.DataMap;
		mapDic = tables.TbMap.DataMap;
		monsterDic = tables.TbMonster.DataMap;
		nameDic = tables.TbName.DataMap;
		skillDic = tables.TbSkill.DataMap;
		skillMoveDic = tables.TbSkillMove.DataMap;
		skillRangeDic = tables.TbSkillRange.DataMap;
		strengthenDic = tables.TbStrengthen.DataMap;
		taskRewardDic = tables.TbTaskReward.DataMap;

		// InitRandomNameConfigFile(PathDefine.RandomNameConfigFiles);
		// InitMonsterConfigFile(PathDefine.MonsterConfigFiles);
		// InitMapConfigFile(PathDefine.MapConfigFiles);
		// InitGuideConfigFile(PathDefine.GuideConfigFiles);
		// InitStrengthenConfigFile(PathDefine.StrengthenConfigFiles);
		// InitTaskConfigFile(PathDefine.TaskConfigFiles);
		// InitSkillConfigFile(PathDefine.SkillConfigFiles);
		// InitSkillMoveConfigFile(PathDefine.SkillMoveConfigFiles);
		// InitSkillActionConfigFile(PathDefine.SkillActionConfigFiles);
		PELogger.Log("资源服务加载完成");
	}
	public GuideConfig GetGuideConfig(int guideId) {
		return guideDic.TryGetValue(guideId, out GuideConfig data) ? data : null;
	}
	public MapConfig GetMapConfig(int mapId) {
		return mapDic.TryGetValue(mapId, out MapConfig data) ? data : null;
	}
	public MonsterConfig GetMonsterConfig(int monsterId) {
		return monsterDic.TryGetValue(monsterId, out MonsterConfig data) ? data : null;
	}
	public Dictionary<int, NameConfig> GetNameConfig() {
		return nameDic;
	}

	public SkillConfig GetSkillConfig(int skillId) {
		return skillDic.TryGetValue(skillId, out SkillConfig data) ? data : null;
	}
	public SkillMoveConfig GetSkillMoveConfig(int skillId) {
		return skillMoveDic.TryGetValue(skillId, out SkillMoveConfig data) ? data : null;
	}
	public SkillRangeConfig GetSkillRangeConfig(int skillId) {
		return skillRangeDic.TryGetValue(skillId, out SkillRangeConfig data) ? data : null;
	}
	public StrengthenConfig GetStrengthenConfig(int strengthenId) {
		return strengthenDic.TryGetValue(strengthenId, out StrengthenConfig data) ? data : null;
	}

	public TaskRewardConfig GetTaskRewardConfig(int taskId) {
		return taskRewardDic.TryGetValue(taskId, out TaskRewardConfig data) ? data : null;
	}

	private static JSONNode JsonLoader(string fileName) {
		return JSON.Parse(File.ReadAllText($"{Application.dataPath}/Resources/ResJsonConfig/Gen/Data/{fileName}.json"));
	}


	#region 工具函数
	public void AsyncLoadScene(string sceneName, Action callBack = null) {
		StartCoroutine(LoadSceneAsync(sceneName, callBack));
	}

	private IEnumerator LoadSceneAsync(string sceneName, Action loaded) {
		GameRoot.Instance.LoadingPanel.SetPanelState();
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		operation.allowSceneActivation = false;
		while(!operation.isDone) {
			float progress = operation.progress;
			if(progress >= 0.9f) {
				operation.allowSceneActivation = true;
				progress = 1.0f;
			}

			GameRoot.Instance.LoadingPanel.SetProgress(progress);
			yield return null;
		}

		loaded?.Invoke();
		GameRoot.Instance.LoadingPanel.SetPanelState(false);
	}


	private readonly Dictionary<string, Sprite> spritesDic = new();

	public Sprite LoadSprite(string path, bool cache = false) {
		if(spritesDic.TryGetValue(path, out Sprite sprite)) return sprite;
		sprite = Resources.Load<Sprite>(path);
		if(cache) spritesDic.Add(path, sprite);

		return sprite;
	}


	private readonly Dictionary<string, AudioClip> audioDictionary = new();

	public AudioClip LoadAudioClip(string path, bool cache = false) {
		if(audioDictionary.TryGetValue(path, out AudioClip audioClip)) return audioClip;
		audioClip = Resources.Load<AudioClip>(path);
		if(cache) audioDictionary.Add(path, audioClip);

		return audioClip;
	}


	private readonly Dictionary<string, GameObject> prefabsDic = new();

	public GameObject LoadPrefab(string path, bool cache = false) {
		if(prefabsDic.TryGetValue(path, out GameObject prefab)) return prefab;
		prefab = Resources.Load<GameObject>(path);
		if(cache) prefabsDic.Add(path, prefab);

		return prefab;
	}
	#endregion

	#region XML
	#region 名字配置读取
	// private readonly Dictionary<int, NameConfig> nameConfigsDic = new();
	//
	// private void InitRandomNameConfigFile(string path) {
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.RandomNameConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new XmlDocument();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	//
	// 		if(nodeList == null) return;
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText != null) {
	// 				int id = int.Parse(innerText);
	//
	// 				NameConfig nameConfig = new NameConfig{
	// 					id = id
	// 				};
	//
	// 				foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 					switch(childNode.Name) {
	// 						case"surname":
	// 							nameConfig.surname = childNode.InnerText;
	// 							break;
	// 						case"man":
	// 							nameConfig.man = childNode.InnerText;
	// 							break;
	// 						case"woman":
	// 							nameConfig.woman = childNode.InnerText;
	// 							break;
	// 					}
	//
	// 				nameConfigsDic.Add(id, nameConfig);
	// 			}
	// 		}
	// 	}
	// }
	//
	// public string GetRandomNameConfig(bool man = true) {
	// 	string randomName =
	// 		nameConfigsDic[Random.Range(0, nameConfigsDic.Count - 1)].surname;
	// 	if(man)
	// 		randomName +=
	// 			nameConfigsDic[Random.Range(0, nameConfigsDic.Count - 1)].man;
	// 	else
	// 		randomName +=
	// 			nameConfigsDic[Random.Range(0, nameConfigsDic.Count - 1)].woman;
	//
	// 	return randomName;
	// }
	#endregion

	#region 怪物配置读取
	// private readonly Dictionary<int, MonsterConfig> monsterConfigsDic = new();
	//
	// private void InitMonsterConfigFile(string path) {
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.MonsterConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new XmlDocument();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	// 		if(nodeList == null) return;
	//
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText != null) {
	// 				int id = int.Parse(innerText);
	//
	// 				MonsterConfig monsterCfg = new(){
	// 					id = id,
	// 					props = new BattleProps()
	// 				};
	//
	// 				foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 					switch(childNode.Name) {
	// 						case"mName":
	// 							monsterCfg.mName = childNode.InnerText;
	// 							break;
	// 						case"mType":
	// 							monsterCfg.mType = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"isStop":
	// 							monsterCfg.isStop = childNode.InnerText.Equals("1");
	// 							break;
	// 						case"resPath":
	// 							monsterCfg.resPath = childNode.InnerText;
	// 							break;
	// 						case"skillID":
	// 							monsterCfg.skillID = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"atkDis":
	// 							monsterCfg.atkDis = float.Parse(childNode.InnerText);
	// 							break;
	// 						case"hp":
	// 							monsterCfg.props.hp = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"ad":
	// 							monsterCfg.props.ad = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"ap":
	// 							monsterCfg.props.ap = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"addef":
	// 							monsterCfg.props.addef = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"apdef":
	// 							monsterCfg.props.apdef = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"dodge":
	// 							monsterCfg.props.dodge = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"pierce":
	// 							monsterCfg.props.pierce = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"critical":
	// 							monsterCfg.props.critical = int.Parse(childNode.InnerText);
	// 							break;
	// 					}
	//
	// 				monsterConfigsDic.Add(id, monsterCfg);
	// 			}
	// 		}
	// 	}
	// }
	//
	// public MonsterConfig GetMonsterConfig(int monsterId) {
	// 	return monsterConfigsDic.TryGetValue(monsterId, out MonsterConfig data) ? data : null;
	// }
	#endregion

	#region 地图配置读取
	// private readonly Dictionary<int, MapConfig> mapConfigsDic = new();
	//
	// private void InitMapConfigFile(string path) {
	// 	
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.MapConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new XmlDocument();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	// 		if(nodeList == null) return;
	// 	
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText != null) {
	// 				int id = int.Parse(innerText);
	// 	
	// 				MapConfig mapConfig = new MapConfig{
	// 					id = id,
	// 					monsterList = new List<MonsterDate>()
	// 				};
	// 	
	// 				foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 					switch(childNode.Name) {
	// 						case"mapName":
	// 							mapConfig.mapName = childNode.InnerText;
	// 							break;
	// 						case"sceneName":
	// 							mapConfig.sceneName = childNode.InnerText;
	// 							break;
	// 						case"power":
	// 							mapConfig.power = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"mainCamPos": {
	// 							string[] valArr = childNode.InnerText.Split(',');
	// 							mapConfig.mainCamPos = new Vector3(float.Parse(valArr[0]),
	// 								float.Parse(valArr[1]), float.Parse(valArr[2]));
	// 						}
	// 							break;
	// 						case"mainCamRote": {
	// 							string[] valArr = childNode.InnerText.Split(',');
	// 							mapConfig.mainCamRote = new Vector3(float.Parse(valArr[0]),
	// 								float.Parse(valArr[1]), float.Parse(valArr[2]));
	// 						}
	// 							break;
	// 						case"playerBornPos": {
	// 							string[] valArr = childNode.InnerText.Split(',');
	// 							mapConfig.playerBornPos = new Vector3(float.Parse(valArr[0]),
	// 								float.Parse(valArr[1]), float.Parse(valArr[2]));
	// 						}
	// 							break;
	// 						case"playerBornRote": {
	// 							string[] valArr = childNode.InnerText.Split(',');
	// 							mapConfig.playerBornRote = new Vector3(float.Parse(valArr[0]),
	// 								float.Parse(valArr[1]), float.Parse(valArr[2]));
	// 						}
	// 							break;
	// 						case"monsterLst": {
	// 							string[] valArr = childNode.InnerText.Split('#');
	// 							for(int waveIndex = 0; waveIndex < valArr.Length; waveIndex++) {
	// 								string wave = valArr[waveIndex];
	// 								if(string.IsNullOrWhiteSpace(wave)) continue;
	// 								string[] monsterArr = wave.Split("|");
	// 								for(int monsterIndex = 0; monsterIndex < monsterArr.Length; monsterIndex++) {
	// 									string monster = monsterArr[monsterIndex];
	// 									if(string.IsNullOrWhiteSpace(monster)) continue;
	// 									string[] infoArr = monster.Split(",");
	// 									MonsterDate monsterDate = new(){
	// 										id = int.Parse(infoArr[0]),
	// 										mWave = waveIndex,
	// 										mIndex = monsterIndex,
	// 										mCfg = GetMonsterConfig(int.Parse(infoArr[0])),
	// 										mBornPos = new Vector3(float.Parse(infoArr[1]), float.Parse(infoArr[2]),
	// 											float.Parse(infoArr[3])),
	// 										mBornRotate = new Vector3(0, float.Parse(infoArr[4]), 0),
	// 										mLevel = int.Parse(infoArr[5])
	// 									};
	// 									mapConfig.monsterList.Add(monsterDate);
	// 								}
	// 							}
	// 						}
	// 							break;
	// 						case"coin":
	// 							mapConfig.Coin = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"exp":
	// 							mapConfig.Exp = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"crystal":
	// 							mapConfig.Crystal = int.Parse(childNode.InnerText);
	// 							break;
	// 					}
	// 	
	// 				mapConfigsDic.Add(id, mapConfig);
	// 			}
	// 		}
	// 	}
	// }
	//
	// public MapConfig GetMapConfig(int mapID) {
	// 	return mapConfigsDic.TryGetValue(mapID, out MapConfig data) ? data : null;
	// }
	#endregion

	#region 引导配置读取
	// private Dictionary<int, GuideConfig> guideConfigsDic = new();
	//
	// private void InitGuideConfigFile(string path) {
	// 	
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new XmlDocument();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	// 	
	// 		if(nodeList == null) return;
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText != null) {
	// 				int id = int.Parse(innerText);
	// 				GuideConfig guideConfig = new GuideConfig{
	// 					id = id
	// 				};
	// 	
	// 				foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 					switch(childNode.Name) {
	// 						case"npcID":
	// 							guideConfig.npcId = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"dilogArr":
	// 							guideConfig.dialogArr = childNode.InnerText;
	// 							break;
	// 						case"actID":
	// 							guideConfig.actionId = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"coin":
	// 							guideConfig.coin = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"exp":
	// 							guideConfig.exp = int.Parse(childNode.InnerText);
	// 							break;
	// 					}
	// 	
	// 				guideConfigsDic.Add(id, guideConfig);
	// 			}
	// 		}
	// 	}
	// }
	//
	// public GuideConfig GetGuideConfig(int guideId) {
	// 	return guideConfigsDic.TryGetValue(guideId, out GuideConfig data) ? data : null;
	// }
	//
	#endregion

	#region 技能配置读取
	// private readonly Dictionary<int, SkillConfig> skillConfigsDic = new();
	//
	// private void InitSkillConfigFile(string path) {
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	//
	// 		if(nodeList == null) return;
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText == null) continue;
	// 			int id = int.Parse(innerText);
	// 			SkillConfig skillConfig = new(){
	// 				id = id,
	// 				skillMoveList = new List<int>(),
	// 				skillActionList = new List<int>(),
	// 				skillDamageList = new List<int>()
	// 			};
	//
	// 			foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 				switch(childNode.Name) {
	// 					case"skillName":
	// 						skillConfig.skillName = childNode.InnerText;
	// 						break;
	// 					case"cdTime":
	// 						skillConfig.cdTime = int.Parse(childNode.InnerText);
	// 						break;
	// 					case"skillTime":
	// 						skillConfig.skillTime = uint.Parse(childNode.InnerText);
	// 						break;
	// 					case"isCombo":
	// 						skillConfig.isCombo = childNode.InnerText.Equals("1");
	// 						break;
	// 					case"isCollide":
	// 						skillConfig.isCollide = childNode.InnerText.Equals("1");
	// 						break;
	// 					case"isBreak":
	// 						skillConfig.isBreak = childNode.InnerText.Equals("1");
	// 						break;
	// 					case"aniAction":
	// 						skillConfig.skillId = int.Parse(childNode.InnerText);
	// 						break;
	// 					case"fx":
	// 						skillConfig.effectName = childNode.InnerText;
	// 						break;
	// 					case"dmgType":
	// 						switch(childNode.InnerText) {
	// 							case"1":
	// 								skillConfig.damageType = DamageType.AD;
	// 								break;
	// 							case"2":
	// 								skillConfig.damageType = DamageType.Ap;
	// 								break;
	// 							default:
	// 								PELogger.Log("DamageType Error.");
	// 								break;
	// 						}
	//
	// 						break;
	// 					case"skillMoveLst":
	// 						string[] skillMoveArr = childNode.InnerText.Split('|');
	// 						foreach(string skillMove in skillMoveArr) {
	// 							if(!string.IsNullOrWhiteSpace(skillMove)) {
	// 								skillConfig.skillMoveList.Add(int.Parse(skillMove));
	// 							}
	// 						}
	//
	// 						break;
	// 					case"skillActionLst":
	// 						string[] skillActionArr = childNode.InnerText.Split('|');
	// 						foreach(string skillAction in skillActionArr) {
	// 							if(!string.IsNullOrWhiteSpace(skillAction)) {
	// 								skillConfig.skillActionList.Add(int.Parse(skillAction));
	// 							}
	// 						}
	//
	// 						break;
	// 					case"skillDamageLst":
	// 						string[] skillDamageArr = childNode.InnerText.Split('|');
	// 						foreach(string skillDamage in skillDamageArr) {
	// 							if(!string.IsNullOrWhiteSpace(skillDamage)) {
	// 								skillConfig.skillDamageList.Add(int.Parse(skillDamage));
	// 							}
	// 						}
	//
	// 						break;
	// 				}
	//
	// 			skillConfigsDic.Add(id, skillConfig);
	// 		}
	// 	}
	// }
	//
	// public SkillConfig GetSkillConfig(int skillId) {
	// 	return skillConfigsDic.TryGetValue(skillId, out SkillConfig data) ? data : null;
	// }
	#endregion

	#region 技能行为读取
	// private readonly Dictionary<int, SkillActionConfig> skillActionConfigDic = new();
	//
	// private void InitSkillActionConfigFile(string path) {
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.SkillActionConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new XmlDocument();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	//
	// 		if(nodeList == null) return;
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(nodeList[i] is not XmlElement element) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText == null) continue;
	// 			int id = int.Parse(innerText);
	// 			SkillActionConfig skillActionConfig = new(){
	// 				id = id,
	// 			};
	//
	// 			foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 				switch(childNode.Name) {
	// 					case"delayTime":
	// 						skillActionConfig.delayTime = uint.Parse(childNode.InnerText);
	// 						break;
	// 					case"radius":
	// 						skillActionConfig.radius = float.Parse(childNode.InnerText);
	// 						break;
	// 					case"angle":
	// 						skillActionConfig.angle = float.Parse(childNode.InnerText);
	// 						break;
	// 				}
	//
	// 			skillActionConfigDic.Add(id, skillActionConfig);
	// 		}
	// 	}
	// }
	//
	// public SkillActionConfig GetSkillActionConfig(int actionId) {
	// 	return skillActionConfigDic.TryGetValue(actionId, out SkillActionConfig data) ? data : null;
	// }
	#endregion

	#region 技能位移配置读取
	// private readonly Dictionary<int, SkillMoveConfig> skillMoveConfigsDic = new();
	//
	// private void InitSkillMoveConfigFile(string path) {
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	//
	// 		if(nodeList == null) return;
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText == null) continue;
	// 			int id = int.Parse(innerText);
	// 			SkillMoveConfig skillMoveConfig = new SkillMoveConfig{
	// 				id = id
	// 			};
	//
	// 			foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 				switch(childNode.Name) {
	// 					case"delayTime":
	// 						skillMoveConfig.delayTime = uint.Parse(childNode.InnerText);
	// 						break;
	// 					case"moveTime":
	// 						skillMoveConfig.moveTime = uint.Parse(childNode.InnerText);
	// 						break;
	// 					case"moveDis":
	// 						skillMoveConfig.moveDis = float.Parse(childNode.InnerText);
	// 						break;
	// 				}
	//
	// 			skillMoveConfigsDic.Add(id, skillMoveConfig);
	// 		}
	// 	}
	// }
	//
	// public SkillMoveConfig GetSkillMoveConfig(int moveId) {
	// 	return skillMoveConfigsDic.TryGetValue(moveId, out SkillMoveConfig data) ? data : null;
	// }
	#endregion

	#region 强化配置读取
	// private readonly Dictionary<int, Dictionary<int, StrengthenConfig>> strengthenConfigsDic = new();
	//
	// private void InitStrengthenConfigFile(string path) {
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file" + PathDefine.StrengthenConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new XmlDocument();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	// 		if(nodeList == null) return;
	//
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	//
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText != null) {
	// 				int id = int.Parse(innerText);
	//
	// 				StrengthenConfig strengthenConfig = new StrengthenConfig{
	// 					id = id
	// 				};
	// 				foreach(XmlElement childNode in nodeList[i].ChildNodes) {
	// 					int value = int.Parse(childNode.InnerText);
	//
	// 					switch(childNode.Name) {
	// 						case"pos":
	// 							strengthenConfig.position = value;
	// 							break;
	// 						case"starlv":
	// 							strengthenConfig.starLevel = value;
	// 							break;
	// 						case"addhp":
	// 							strengthenConfig.addHp = value;
	// 							break;
	// 						case"addhurt":
	// 							strengthenConfig.addHurt = value;
	// 							break;
	// 						case"adddef":
	// 							strengthenConfig.addDefine = value;
	// 							break;
	// 						case"minlv":
	// 							strengthenConfig.minLevel = value;
	// 							break;
	// 						case"coin":
	// 							strengthenConfig.coin = value;
	// 							break;
	// 						case"crystal":
	// 							strengthenConfig.crystal = value;
	// 							break;
	// 					}
	// 				}
	//
	// 				if(strengthenConfigsDic.TryGetValue(strengthenConfig.position,
	// 					   out Dictionary<int, StrengthenConfig> starLevelDic)) {
	// 					starLevelDic.Add(strengthenConfig.starLevel, strengthenConfig);
	// 				}
	// 				else {
	// 					starLevelDic = new Dictionary<int, StrengthenConfig>{
	// 						{ strengthenConfig.starLevel, strengthenConfig }
	// 					};
	// 					strengthenConfigsDic.Add(strengthenConfig.position, starLevelDic);
	// 				}
	// 			}
	// 		}
	// 	}
	// }
	//
	// public StrengthenConfig GetStrengthenConfig(int position, int starLevel) {
	// 	if(!strengthenConfigsDic.TryGetValue(position,
	// 		   out var starLevelDic))
	// 		return null;
	//
	// 	return starLevelDic.TryGetValue(starLevel, out StrengthenConfig strengthenData)
	// 		? strengthenData
	// 		: null;
	// }
	#endregion

	#region 任务配置读取
	// private readonly Dictionary<int, TaskConfig> taskConfigsDic = new();
	//
	// private void InitTaskConfigFile(string path) {
	// 	TextAsset xml = Resources.Load<TextAsset>(path);
	// 	if(!xml) {
	// 		PELogger.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", LogColor.Red);
	// 	}
	// 	else {
	// 		XmlDocument document = new XmlDocument();
	// 		document.LoadXml(xml.text);
	// 		XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
	//
	// 		if(nodeList == null) return;
	// 		for(int i = 0; i < nodeList.Count; i++) {
	// 			if(!(nodeList[i] is XmlElement element)) continue;
	// 			string innerText = element.GetAttributeNode("ID")?.InnerText;
	// 			if(innerText != null) {
	// 				int id = int.Parse(innerText);
	// 				TaskConfig taskConfig = new TaskConfig{
	// 					id = id
	// 				};
	//
	// 				foreach(XmlElement childNode in nodeList[i].ChildNodes)
	// 					switch(childNode.Name) {
	// 						case"taskName":
	// 							taskConfig.taskName = childNode.InnerText;
	// 							break;
	// 						case"count":
	// 							taskConfig.targetCount = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"exp":
	// 							taskConfig.exp = int.Parse(childNode.InnerText);
	// 							break;
	// 						case"coin":
	// 							taskConfig.coin = int.Parse(childNode.InnerText);
	// 							break;
	// 					}
	//
	// 				taskConfigsDic.Add(id, taskConfig);
	// 			}
	// 		}
	// 	}
	// }
	//
	// public TaskConfig GetTaskConfig(int taskId) {
	// 	return taskConfigsDic.TryGetValue(taskId, out TaskConfig data) ? data : null;
	// }
	#endregion


// #if UNITY_EDITOR
// 	public void ResetSkillConfigFiles() {
// 		skillConfigsDic.Clear();
// 		skillMoveConfigsDic.Clear();
// 		InitSkillConfigFile(PathDefine.SkillConfigFiles);
// 		InitSkillMoveConfigFile(PathDefine.SkillMoveConfigFiles);
// 		PELogger.Log("重新加载技能配置");
// 	}
// #endif
	#endregion
}