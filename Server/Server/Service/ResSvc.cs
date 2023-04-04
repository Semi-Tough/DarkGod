/****************************************************
    文件：ConfigService
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月10日 星期四 11:10
    功能：配置服务
*****************************************************/

using cfg;
using PETool.PELogger;
using System.Text.Json;

namespace Sever {
	public class ResSvc {
		public static ResSvc Instance { get; } = new ResSvc();


		public Dictionary<int, GuideConfig> guideDic = new();
		public Dictionary<int, StrengthenConfig> strengthenDic = new();
		public Dictionary<int, TaskRewardConfig> taskRewardDic = new();
		public Dictionary<int, MapConfig> mapDic = new();
		public void Init() {
			// InitGuideConfigFile();
			// InitStrengthenConfigFile();
			// InitTaskConfigFile();
			// InitMapConfigFile();
			Tables tables = new Tables(LoadJson);
			guideDic = tables.TbGuide.DataMap;
			strengthenDic = tables.TbStrengthen.DataMap;
			taskRewardDic = tables.TbTaskReward.DataMap;
			mapDic = tables.TbMap.DataMap;


			PELogger.Log("ConfigService Init Done.");
		}
		public GuideConfig? GetGuideConfig(int guideId) {
			guideDic.TryGetValue(guideId, out GuideConfig? data);
			return data;
		}
		public StrengthenConfig? GetStrengthenConfig(int position, int starLevel) {
			strengthenDic.TryGetValue(position * 10 + starLevel, out StrengthenConfig? data);
			return data;
		}
		public TaskRewardConfig? GetTaskRewardConfig(int guideId) {
			taskRewardDic.TryGetValue(guideId, out TaskRewardConfig? data);
			return data;
		}
		public MapConfig? GetMapConfig(int guideId) {
			mapDic.TryGetValue(guideId, out MapConfig? data);
			return data;
		}

		private JsonElement LoadJson(string fileName) {
			return JsonDocument.Parse(File.ReadAllBytes($"../../../Luban/Config/Gen/Data/{fileName}.json")).RootElement;
		}

		#region XML
		#region 引导配置
		// private readonly Dictionary<int, GuideConfig> guideConfigsDic = new Dictionary<int, GuideConfig>();
		//
		// private void InitGuideConfigFile() {
		// 	XmlDocument document = new XmlDocument();
		//
		// 	document.Load("XMLConfig/Guide.xml");
		//
		// 	XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
		// 	if(nodeList == null) return;
		//
		// 	for(int i = 0; i < nodeList.Count; i++) {
		// 		if(!(nodeList[i] is XmlElement element)) continue;
		// 		string innerText = element.GetAttributeNode("ID")?.InnerText;
		// 		if(innerText == null) continue;
		// 		int id = int.Parse(innerText);
		//
		// 		GuideConfig guideConfig = new GuideConfig{
		// 			id = id
		// 		};
		//
		// 		foreach(XmlElement childNode in nodeList[i].ChildNodes)
		// 			switch(childNode.Name) {
		// 				case"coin":
		// 					guideConfig.coin = int.Parse(childNode.InnerText);
		// 					break;
		// 				case"exp":
		// 					guideConfig.exp = int.Parse(childNode.InnerText);
		// 					break;
		// 			}
		//
		// 		guideConfigsDic.Add(id, guideConfig);
		// 	}
		//
		// 	PELogger.Log("GuideConfigFile Init Done.");
		// }
		//
		// public GuideConfig GetGuideConfig(int guideId) {
		// 	return guideConfigsDic.TryGetValue(guideId, out GuideConfig guideConfig)
		// 		? guideConfig
		// 		: null;
		// }
		#endregion

		#region 强化配置
		// private readonly Dictionary<int, Dictionary<int, StrengthenConfig>> strengthenPositionDic =
		// 	new Dictionary<int, Dictionary<int, StrengthenConfig>>();
		//
		// private void InitStrengthenConfigFile() {
		// 	XmlDocument document = new XmlDocument();
		// 	document.Load("XMLConfig/Strengthen.xml");
		//
		// 	XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
		// 	if(nodeList == null) return;
		//
		// 	for(int i = 0; i < nodeList.Count; i++) {
		// 		if(!(nodeList[i] is XmlElement element)) continue;
		// 		string innerText = element.GetAttributeNode("ID")?.InnerText;
		// 		if(innerText != null) {
		// 			int id = int.Parse(innerText);
		//
		// 			StrengthenConfig strengthenConfig = new StrengthenConfig{
		// 				id = id
		// 			};
		//
		// 			foreach(XmlElement childNode in nodeList[i].ChildNodes) {
		// 				int value = int.Parse(childNode.InnerText);
		// 				switch(childNode.Name) {
		// 					case"pos":
		// 						strengthenConfig.position = value;
		// 						break;
		// 					// ReSharper disable once StringLiteralTypo
		// 					case"starlv":
		// 						strengthenConfig.starLevel = value;
		// 						break;
		// 					// ReSharper disable once StringLiteralTypo
		// 					case"addhp":
		// 						strengthenConfig.addHp = value;
		// 						break;
		// 					// ReSharper disable once StringLiteralTypo
		// 					case"adddef":
		// 						strengthenConfig.addDefense = value;
		// 						break;
		// 					// ReSharper disable once StringLiteralTypo
		//
		// 					case"addhurt":
		// 						strengthenConfig.addHurt = value;
		// 						break;
		// 					// ReSharper disable once StringLiteralTypo
		// 					case"minlv":
		// 						strengthenConfig.minLevel = value;
		// 						break;
		// 					case"coin":
		// 						strengthenConfig.coin = value;
		// 						break;
		// 					case"crystal":
		// 						strengthenConfig.crystal = value;
		// 						break;
		// 				}
		// 			}
		//
		// 			if(strengthenPositionDic.TryGetValue(strengthenConfig.position,
		// 				   out Dictionary<int, StrengthenConfig> starLevelDic)) {
		// 				starLevelDic.Add(strengthenConfig.starLevel, strengthenConfig);
		// 			}
		// 			else {
		// 				starLevelDic = new Dictionary<int, StrengthenConfig>{
		// 					{ strengthenConfig.starLevel, strengthenConfig }
		// 				};
		// 				strengthenPositionDic.Add(strengthenConfig.position, starLevelDic);
		// 			}
		// 		}
		// 	}
		//
		// 	PELogger.Log("StrengthenConfigFile Init Done.");
		// }
		//
		//
		// public StrengthenConfig GetStrengthenData(int position, int starLevel) {
		// 	if(!strengthenPositionDic.TryGetValue(position,
		// 		   out Dictionary<int, StrengthenConfig> starLevelDic)) return null;
		//
		// 	return starLevelDic.TryGetValue(starLevel, out StrengthenConfig strengthenData)
		// 		? strengthenData
		// 		: null;
		// }
		#endregion

		#region 任务奖励配置
		// private readonly Dictionary<int, TaskConfig> taskConfigsDic = new Dictionary<int, TaskConfig>();
		//
		// private void InitTaskConfigFile() {
		// 	XmlDocument document = new XmlDocument();
		//
		// 	document.Load("XMLConfig/Task.xml");
		//
		// 	XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
		// 	if(nodeList == null) return;
		//
		// 	for(int i = 0; i < nodeList.Count; i++) {
		// 		if(!(nodeList[i] is XmlElement element)) continue;
		// 		string innerText = element.GetAttributeNode("ID")?.InnerText;
		// 		if(innerText != null) {
		// 			int id = int.Parse(innerText);
		//
		// 			TaskConfig taskConfig = new TaskConfig{
		// 				id = id
		// 			};
		//
		// 			foreach(XmlElement childNode in nodeList[i].ChildNodes)
		// 				switch(childNode.Name) {
		// 					case"count":
		// 						taskConfig.targetCount = int.Parse(childNode.InnerText);
		// 						break;
		// 					case"coin":
		// 						taskConfig.coin = int.Parse(childNode.InnerText);
		// 						break;
		// 					case"exp":
		// 						taskConfig.exp = int.Parse(childNode.InnerText);
		// 						break;
		// 				}
		//
		// 			taskConfigsDic.Add(id, taskConfig);
		// 		}
		// 	}
		//
		// 	PELogger.Log("TaskConfigFile Init Done.");
		// }
		//
		// public TaskConfig GetTaskConfig(int taskId) {
		// 	return taskConfigsDic.TryGetValue(taskId, out TaskConfig taskConfig)
		// 		? taskConfig
		// 		: null;
		// }
		#endregion

		#region 地图配置
		// private readonly Dictionary<int, MapConfig> mapConfigsDic = new Dictionary<int, MapConfig>();
		//
		// private void InitMapConfigFile() {
		// 	XmlDocument document = new XmlDocument();
		// 	document.Load("XMLConfig/Map.xml");
		//
		//
		// 	XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
		// 	if(nodeList == null) return;
		//
		// 	for(int i = 0; i < nodeList.Count; i++) {
		// 		if(!(nodeList[i] is XmlElement element)) continue;
		// 		string innerText = element.GetAttributeNode("ID")?.InnerText;
		// 		if(innerText == null) continue;
		// 		int id = int.Parse(innerText);
		//
		// 		MapConfig mapConfig = new MapConfig(){
		// 			id = id
		// 		};
		//
		// 		foreach(XmlElement childNode in nodeList[i].ChildNodes)
		// 			switch(childNode.Name) {
		// 				case"power":
		// 					mapConfig.power = int.Parse(childNode.InnerText);
		// 					break;
		// 				case"coin":
		// 					mapConfig.coin = int.Parse(childNode.InnerText);
		// 					break;
		// 				case"exp":
		// 					mapConfig.exp = int.Parse(childNode.InnerText);
		// 					break;
		// 				case"crystal":
		// 					mapConfig.crystal = int.Parse(childNode.InnerText);
		// 					break;
		// 			}
		//
		// 		mapConfigsDic.Add(id, mapConfig);
		// 	}
		//
		// 	PELogger.Log("mapConfigFile Init Done.");
		// }
		//
		// public MapConfig GetMapConfig(int mapId) {
		// 	return mapConfigsDic.TryGetValue(mapId, out MapConfig mapConfig)
		// 		? mapConfig
		// 		: null;
		// }
		#endregion
		#endregion
	}
}