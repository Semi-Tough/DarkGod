/****************************************************
    文件：ResSvc.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/22 12:44:44
    功能：资源服务
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using PEProtocol;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class ResourceService : MonoBehaviour
{
    public static ResourceService instance;

    public void InitService()
    {
        instance = this;
        InitRandomNameConfigFile(PathDefine.RandomNameConfigFiles);
        InitMapConfigFile(PathDefine.MapConfigFiles);
        InitGuideConfigFile(PathDefine.GuideConfigFiles);
        InitStrengthenConfigFile(PathDefine.StrengthenConfigFiles);
        InitTaskConfigFile(PathDefine.TaskConfigFiles);
        InitSkillConfigFile(PathDefine.SkillConfigFiles);
        InitSkillMoveConfigFile(PathDefine.SkillMoveConfigFiles);

        PeCommon.Log("资源服务加载完成");
    }

    #region 工具函数

    #region 加载场景

    public IEnumerator AsyncLoadScene(string sceneName, Action loaded)
    {
        GameRoot.instance.loadingWindow.SetWindowState();
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        sceneAsync.allowSceneActivation = false;
        while (!sceneAsync.isDone)
        {
            GameRoot.instance.loadingWindow.SetProgress(sceneAsync.progress);

            if (sceneAsync.progress >= 0.9f) sceneAsync.allowSceneActivation = true;

            yield return null;
        }

        loaded?.Invoke();
        GameRoot.instance.loadingWindow.SetWindowState(false);
    }

    #endregion

    #region 加载图片

    private readonly Dictionary<string, Sprite> _spritesDic = new Dictionary<string, Sprite>();

    public Sprite LoadSprite(string path, bool cache = false)
    {
        if (_spritesDic.TryGetValue(path, out Sprite sprite)) return sprite;
        sprite = Resources.Load<Sprite>(path);
        if (cache) _spritesDic.Add(path, sprite);

        return sprite;
    }

    #endregion

    #region 加载预设

    private readonly Dictionary<string, GameObject> _prefabsDic =
        new Dictionary<string, GameObject>();

    public GameObject LoadPrefab(string path, bool cache = false)
    {
        if (_prefabsDic.TryGetValue(path, out GameObject prefab)) return prefab;
        prefab = Resources.Load<GameObject>(path);
        if (cache) _prefabsDic.Add(path, prefab);

        return prefab;
    }

    #endregion

    #region 加载音频片段

    private readonly Dictionary<string, AudioClip> _audioDictionary =
        new Dictionary<string, AudioClip>();

    public AudioClip LoadAudioClip(string path, bool cache = false)
    {
        if (_audioDictionary.TryGetValue(path, out AudioClip audioClip)) return audioClip;
        audioClip = Resources.Load<AudioClip>(path);
        if (cache) _audioDictionary.Add(path, audioClip);

        return audioClip;
    }

    #endregion

    #endregion

    #region 配置文件

    #region 名字配置读取

    private readonly Dictionary<int, NameConfig> _nameConfigsDic =
        new Dictionary<int, NameConfig>();

    private void InitRandomNameConfigFile(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PeCommon.Log("xml file:" + PathDefine.RandomNameConfigFiles + "not exist", PeLogType.Error);
        }
        else
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml.text);
            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;

            if (nodeList == null) return;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText != null)
                {
                    int id = int.Parse(innerText);

                    NameConfig nameConfig = new NameConfig
                    {
                        id = id
                    };

                    foreach (XmlElement childNode in nodeList[i].ChildNodes)
                        switch (childNode.Name)
                        {
                            case "surname":
                                nameConfig.surname = childNode.InnerText;
                                break;
                            case "man":
                                nameConfig.man = childNode.InnerText;
                                break;
                            case "woman":
                                nameConfig.woman = childNode.InnerText;
                                break;
                        }

                    _nameConfigsDic.Add(id, nameConfig);
                }
            }
        }
    }

    public string GetRandomNameConfig(bool man = true)
    {
        Random random = new Random();
        string randomName =
            _nameConfigsDic[PeTools.RandomInt(0, _nameConfigsDic.Count - 1, random)]
                .surname;
        if (man)
            randomName +=
                _nameConfigsDic[PeTools.RandomInt(0, _nameConfigsDic.Count - 1, random)]
                    .man;
        else
            randomName +=
                _nameConfigsDic[PeTools.RandomInt(0, _nameConfigsDic.Count - 1, random)]
                    .woman;

        return randomName;
    }

    #endregion

    #region 地图配置读取

    private readonly Dictionary<int, MapConfig> _mapConfigsDic = new Dictionary<int, MapConfig>();

    private void InitMapConfigFile(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PeCommon.Log("xml file:" + PathDefine.MapConfigFiles + "not exist", PeLogType.Error);
        }
        else
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml.text);
            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;
            if (nodeList == null) return;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText != null)
                {
                    int id = int.Parse(innerText);

                    MapConfig mapConfig = new MapConfig
                    {
                        id = id
                    };

                    foreach (XmlElement childNode in nodeList[i].ChildNodes)
                        switch (childNode.Name)
                        {
                            case "mapName":
                                mapConfig.mapName = childNode.InnerText;
                                break;
                            case "sceneName":
                                mapConfig.sceneName = childNode.InnerText;
                                break;
                            case "power":
                                mapConfig.power = int.Parse(childNode.InnerText);
                                break;
                            case "mainCamPos":
                            {
                                string[] valArr = childNode.InnerText.Split(',');
                                mapConfig.mainCamPos = new Vector3(float.Parse(valArr[0]),
                                    float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                                break;
                            case "mainCamRote":
                            {
                                string[] valArr = childNode.InnerText.Split(',');
                                mapConfig.mainCamRote = new Vector3(float.Parse(valArr[0]),
                                    float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                                break;
                            case "playerBornPos":
                            {
                                string[] valArr = childNode.InnerText.Split(',');
                                mapConfig.playerBornPos = new Vector3(float.Parse(valArr[0]),
                                    float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                                break;
                            case "playerBornRote":
                            {
                                string[] valArr = childNode.InnerText.Split(',');
                                mapConfig.playerBornRote = new Vector3(float.Parse(valArr[0]),
                                    float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                                break;
                        }

                    _mapConfigsDic.Add(id, mapConfig);
                }
            }
        }
    }

    public MapConfig GetMapConfig(int mapID)
    {
        return _mapConfigsDic.TryGetValue(mapID, out MapConfig data) ? data : null;
    }

    #endregion

    #region 引导配置读取

    private readonly Dictionary<int, GuideConfig> _guideConfigsDic = new Dictionary<int, GuideConfig>();

    private void InitGuideConfigFile(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PeCommon.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", PeLogType.Error);
        }
        else
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml.text);
            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;

            if (nodeList == null) return;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText != null)
                {
                    int id = int.Parse(innerText);
                    GuideConfig guideConfig = new GuideConfig
                    {
                        id = id
                    };

                    foreach (XmlElement childNode in nodeList[i].ChildNodes)
                        switch (childNode.Name)
                        {
                            case "npcID":
                                guideConfig.npcId = int.Parse(childNode.InnerText);
                                break;
                            case "dialogArr":
                                guideConfig.dialogArr = childNode.InnerText;
                                break;
                            case "actID":
                                guideConfig.actionId = int.Parse(childNode.InnerText);
                                break;
                            case "coin":
                                guideConfig.coin = int.Parse(childNode.InnerText);
                                break;
                            case "exp":
                                guideConfig.exp = int.Parse(childNode.InnerText);
                                break;
                        }

                    _guideConfigsDic.Add(id, guideConfig);
                }
            }
        }
    }

    public GuideConfig GetGuideConfig(int guideId)
    {
        return _guideConfigsDic.TryGetValue(guideId, out GuideConfig data) ? data : null;
    }

    #endregion

    #region 强化配置读取

    private readonly Dictionary<int, Dictionary<int, StrengthenConfig>> _strengthenConfigsDic =
        new Dictionary<int, Dictionary<int, StrengthenConfig>>();

    private void InitStrengthenConfigFile(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PeCommon.Log("xml file" + PathDefine.StrengthenConfigFiles + "not exist", PeLogType.Error);
        }
        else
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml.text);
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
                        id = id
                    };
                    foreach (XmlElement childNode in nodeList[i].ChildNodes)
                    {
                        int value = int.Parse(childNode.InnerText);

                        switch (childNode.Name)
                        {
                            case "pos":
                                strengthenConfig.position = value;
                                break;
                            case "starlv":
                                strengthenConfig.starLevel = value;
                                break;
                            case "addhp":
                                strengthenConfig.addHp = value;
                                break;
                            case "addhurt":
                                strengthenConfig.addHurt = value;
                                break;
                            case "adddef":
                                strengthenConfig.addDefine = value;
                                break;
                            case "minlv":
                                strengthenConfig.minLevel = value;
                                break;
                            case "coin":
                                strengthenConfig.coin = value;
                                break;
                            case "crystal":
                                strengthenConfig.crystal = value;
                                break;
                        }
                    }

                    if (_strengthenConfigsDic.TryGetValue(strengthenConfig.position,
                            out Dictionary<int, StrengthenConfig> starLevelDic))
                    {
                        starLevelDic.Add(strengthenConfig.starLevel, strengthenConfig);
                    }
                    else
                    {
                        starLevelDic = new Dictionary<int, StrengthenConfig>
                        {
                            {strengthenConfig.starLevel, strengthenConfig}
                        };
                        _strengthenConfigsDic.Add(strengthenConfig.position, starLevelDic);
                    }
                }
            }
        }
    }

    public StrengthenConfig GetStrengthenConfig(int position, int starLevel)
    {
        if (!_strengthenConfigsDic.TryGetValue(position,
                out var starLevelDic)) return null;

        return starLevelDic.TryGetValue(starLevel, out StrengthenConfig strengthenData)
            ? strengthenData
            : null;
    }

    public enum AddPropertyType
    {
        Hp,
        Defense,
        Hurt
    }

    public int GetCurrentPropertyValue(int position, int starLevel,
        AddPropertyType addPropertyType)
    {
        int result = 0;
        if (_strengthenConfigsDic.TryGetValue(position,
                out var starLevelDic))
            for (int i = 0; i < starLevel; i++)
                if (starLevelDic.TryGetValue(starLevel, out StrengthenConfig strengthenData))
                    switch (addPropertyType)
                    {
                        case AddPropertyType.Hp:
                            result += strengthenData.addHp;
                            break;
                        case AddPropertyType.Defense:
                            result += strengthenData.addDefine;
                            break;
                        case AddPropertyType.Hurt:
                            result += strengthenData.addHurt;
                            break;
                    }

        return result;
    }

    #endregion

    #region 任务配置读取

    private readonly Dictionary<int, TaskConfig> _taskConfigsDic = new Dictionary<int, TaskConfig>();

    private void InitTaskConfigFile(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PeCommon.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", PeLogType.Error);
        }
        else
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml.text);
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
                        id = id
                    };

                    foreach (XmlElement childNode in nodeList[i].ChildNodes)
                        switch (childNode.Name)
                        {
                            case "taskName":
                                taskConfig.taskName = childNode.InnerText;
                                break;
                            case "count":
                                taskConfig.targetCount = int.Parse(childNode.InnerText);
                                break;
                            case "exp":
                                taskConfig.exp = int.Parse(childNode.InnerText);
                                break;
                            case "coin":
                                taskConfig.coin = int.Parse(childNode.InnerText);
                                break;
                        }

                    _taskConfigsDic.Add(id, taskConfig);
                }
            }
        }
    }

    public TaskConfig GetTaskConfig(int guideId)
    {
        return _taskConfigsDic.TryGetValue(guideId, out TaskConfig data) ? data : null;
    }

    #endregion

    #region 技能配置读取

    private readonly Dictionary<int, SkillConfig> _skillConfigsDic = new Dictionary<int, SkillConfig>();

    private void InitSkillConfigFile(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PeCommon.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", PeLogType.Error);
        }
        else
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml.text);
            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;

            if (nodeList == null) return;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText == null) continue;
                int id = int.Parse(innerText);
                SkillConfig skillConfig = new SkillConfig
                {
                    id = id,
                    skillMoveList = new List<int>()
                };

                foreach (XmlElement childNode in nodeList[i].ChildNodes)
                    switch (childNode.Name)
                    {
                        case "skillName":
                            skillConfig.skillName = childNode.InnerText;
                            break;
                        case "skillTime":
                            skillConfig.skillTime = int.Parse(childNode.InnerText);
                            break;
                        case "aniAction":
                            skillConfig.skillId = int.Parse(childNode.InnerText);
                            break;
                        case "fx":
                            skillConfig.effectName = childNode.InnerText;
                            break;
                        case "skillMove":
                            string[] skillMoveArr = childNode.InnerText.Split('|');
                            foreach (string str in skillMoveArr)
                            {
                                if (!string.IsNullOrWhiteSpace(str))
                                {
                                    skillConfig.skillMoveList.Add(int.Parse(str));
                                }
                            }

                            break;
                    }

                _skillConfigsDic.Add(id, skillConfig);
            }
        }
    }

    public SkillConfig GetSkillConfig(int guideId)
    {
        return _skillConfigsDic.TryGetValue(guideId, out SkillConfig data) ? data : null;
    }

    #endregion

    #region 技能位移配置读取

    private readonly Dictionary<int, SkillMoveConfig> _skillMoveConfigsDic = new Dictionary<int, SkillMoveConfig>();

    private void InitSkillMoveConfigFile(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml)
        {
            PeCommon.Log("xml file:" + PathDefine.GuideConfigFiles + "not exist", PeLogType.Error);
        }
        else
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml.text);
            XmlNodeList nodeList = document.SelectSingleNode("root")?.ChildNodes;

            if (nodeList == null) return;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (!(nodeList[i] is XmlElement element)) continue;
                string innerText = element.GetAttributeNode("ID")?.InnerText;
                if (innerText == null) continue;
                int id = int.Parse(innerText);
                SkillMoveConfig skillMoveConfig = new SkillMoveConfig()
                {
                    id = id
                };

                foreach (XmlElement childNode in nodeList[i].ChildNodes)
                    switch (childNode.Name)
                    {
                        case "delayTime":
                            skillMoveConfig.delayTime = int.Parse(childNode.InnerText);
                            break;
                        case "moveTime":
                            skillMoveConfig.moveTime = int.Parse(childNode.InnerText);
                            break;
                        case "moveDis":
                            skillMoveConfig.moveDis = float.Parse(childNode.InnerText);
                            break;
                    }

                _skillMoveConfigsDic.Add(id, skillMoveConfig);
            }
        }
    }

    public SkillMoveConfig GetSkillMoveConfig(int guideId)
    {
        return _skillMoveConfigsDic.TryGetValue(guideId, out SkillMoveConfig data) ? data : null;
    }

    #endregion

#if UNITY_EDITOR

    public void ResetSkillConfigFiles()
    {
        _skillConfigsDic.Clear();
        _skillMoveConfigsDic.Clear();
        InitSkillConfigFile(PathDefine.SkillConfigFiles);
        InitSkillMoveConfigFile(PathDefine.SkillMoveConfigFiles);
        PeCommon.Log("重新加载技能配置");
    }

#endif

    #endregion
}