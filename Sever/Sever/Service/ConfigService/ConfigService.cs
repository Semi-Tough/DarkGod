/****************************************************
    文件：ConfigService
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月10日 星期四 11:10
    功能：配置服务
*****************************************************/

using System.Collections.Generic;
using System.Xml;
using PEProtocol;

namespace Sever
{
    public class ConfigService
    {
        private static ConfigService _instance;
        public static ConfigService Instance => _instance ?? (_instance = new ConfigService());

        public void Init()
        {
            InitGuideConfigFile();
            InitStrengthenConfigFile();
            InitTaskConfigFile();
            InitMapConfigFile();
            PeCommon.Log("ConfigService Init Done.");
        }

        #region 引导配置

        private readonly Dictionary<int, GuideConfig> _guideConfigsDic = new Dictionary<int, GuideConfig>();

        private void InitGuideConfigFile()
        {
            XmlDocument document = new XmlDocument();

            document.Load("../../../XMLConfig/Guide.xml");

            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
            if (nodeList == null) return;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText == null) continue;
                int id = int.Parse(innerText);

                GuideConfig guideConfig = new GuideConfig
                {
                    Id = id
                };

                foreach (XmlElement childNode in nodeList[i].ChildNodes)
                    switch (childNode.Name)
                    {
                        case "coin":
                            guideConfig.Coin = int.Parse(childNode.InnerText);
                            break;
                        case "exp":
                            guideConfig.Exp = int.Parse(childNode.InnerText);
                            break;
                    }

                _guideConfigsDic.Add(id, guideConfig);
            }

            PeCommon.Log("GuideConfigFile Init Done.");
        }

        public GuideConfig GetGuideConfig(int guideId)
        {
            return _guideConfigsDic.TryGetValue(guideId, out GuideConfig guideConfig)
                ? guideConfig
                : null;
        }

        #endregion

        #region 强化配置

        private readonly Dictionary<int, Dictionary<int, StrengthenConfig>> _strengthenPositionDic =
            new Dictionary<int, Dictionary<int, StrengthenConfig>>();

        private void InitStrengthenConfigFile()
        {
            XmlDocument document = new XmlDocument();
            document.Load("../../../XMLConfig/Strengthen.xml");

            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
            if (nodeList == null) return;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText != null)
                {
                    int id = int.Parse(innerText);

                    StrengthenConfig strengthenConfig = new StrengthenConfig
                    {
                        Id = id
                    };

                    foreach (XmlElement childNode in nodeList[i].ChildNodes)
                    {
                        int value = int.Parse(childNode.InnerText);
                        switch (childNode.Name)
                        {
                            case "pos":
                                strengthenConfig.Position = value;
                                break;
                            // ReSharper disable once StringLiteralTypo
                            case "starlv":
                                strengthenConfig.StarLevel = value;
                                break;
                            // ReSharper disable once StringLiteralTypo
                            case "addhp":
                                strengthenConfig.AddHp = value;
                                break;
                            // ReSharper disable once StringLiteralTypo
                            case "adddef":
                                strengthenConfig.AddDefense = value;
                                break;
                            // ReSharper disable once StringLiteralTypo

                            case "addhurt":
                                strengthenConfig.AddHurt = value;
                                break;
                            // ReSharper disable once StringLiteralTypo
                            case "minlv":
                                strengthenConfig.MinLevel = value;
                                break;
                            case "coin":
                                strengthenConfig.Coin = value;
                                break;
                            case "crystal":
                                strengthenConfig.Crystal = value;
                                break;
                        }
                    }

                    if (_strengthenPositionDic.TryGetValue(strengthenConfig.Position,
                            out Dictionary<int, StrengthenConfig> starLevelDic))
                    {
                        starLevelDic.Add(strengthenConfig.StarLevel, strengthenConfig);
                    }
                    else
                    {
                        starLevelDic = new Dictionary<int, StrengthenConfig>
                        {
                            {strengthenConfig.StarLevel, strengthenConfig}
                        };
                        _strengthenPositionDic.Add(strengthenConfig.Position, starLevelDic);
                    }
                }
            }

            PeCommon.Log("StrengthenConfigFile Init Done.");
        }


        public StrengthenConfig GetStrengthenData(int position, int starLevel)
        {
            if (!_strengthenPositionDic.TryGetValue(position,
                    out Dictionary<int, StrengthenConfig> starLevelDic)) return null;

            return starLevelDic.TryGetValue(starLevel, out StrengthenConfig strengthenData)
                ? strengthenData
                : null;
        }

        #endregion

        #region 任务奖励配置

        private readonly Dictionary<int, TaskConfig> _taskConfigsDic = new Dictionary<int, TaskConfig>();

        private void InitTaskConfigFile()
        {
            XmlDocument document = new XmlDocument();

            document.Load("../../../XMLConfig/Task.xml");

            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
            if (nodeList == null) return;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText != null)
                {
                    int id = int.Parse(innerText);

                    TaskConfig taskConfig = new TaskConfig
                    {
                        Id = id
                    };

                    foreach (XmlElement childNode in nodeList[i].ChildNodes)
                        switch (childNode.Name)
                        {
                            case "count":
                                taskConfig.TargetCount = int.Parse(childNode.InnerText);
                                break;
                            case "coin":
                                taskConfig.Coin = int.Parse(childNode.InnerText);
                                break;
                            case "exp":
                                taskConfig.Exp = int.Parse(childNode.InnerText);
                                break;
                        }

                    _taskConfigsDic.Add(id, taskConfig);
                }
            }

            PeCommon.Log("TaskConfigFile Init Done.");
        }

        public TaskConfig GetTaskConfig(int taskId)
        {
            return _taskConfigsDic.TryGetValue(taskId, out TaskConfig taskConfig)
                ? taskConfig
                : null;
        }

        #endregion

        #region 地图配置

        private readonly Dictionary<int, MapConfig> _mapConfigsDic = new Dictionary<int, MapConfig>();

        private void InitMapConfigFile()
        {
            XmlDocument document = new XmlDocument();
            document.Load("../../../XMLConfig/Map.xml");


            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
            if (nodeList == null) return;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText == null) continue;
                int id = int.Parse(innerText);

                MapConfig mapConfig = new MapConfig()
                {
                    Id = id
                };

                foreach (XmlElement childNode in nodeList[i].ChildNodes)
                    switch (childNode.Name)
                    {
                        case "power":
                            mapConfig.Power = int.Parse(childNode.InnerText);
                            break;
                    }

                _mapConfigsDic.Add(id, mapConfig);
            }

            PeCommon.Log("mapConfigFile Init Done.");
        }

        public MapConfig GetMapConfig(int mapId)
        {
            return _mapConfigsDic.TryGetValue(mapId, out MapConfig mapConfig)
                ? mapConfig
                : null;
        }

        #endregion
    }
}