//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using System.Text.Json;



namespace cfg
{

public sealed partial class MapConfig :  Bright.Config.BeanBase 
{
    public MapConfig(JsonElement _json) 
    {
        Id = _json.GetProperty("id").GetInt32();
        MapName = _json.GetProperty("mapName").GetString();
        SceneName = _json.GetProperty("sceneName").GetString();
        Power = _json.GetProperty("power").GetInt32();
        { var _json0 = _json.GetProperty("mainCamPos"); float __x; __x = _json0.GetProperty("x").GetSingle(); float __y; __y = _json0.GetProperty("y").GetSingle(); float __z; __z = _json0.GetProperty("z").GetSingle();  MainCamPos = new System.Numerics.Vector3(__x, __y,__z); }
        { var _json0 = _json.GetProperty("mainCamRote"); float __x; __x = _json0.GetProperty("x").GetSingle(); float __y; __y = _json0.GetProperty("y").GetSingle(); float __z; __z = _json0.GetProperty("z").GetSingle();  MainCamRote = new System.Numerics.Vector3(__x, __y,__z); }
        { var _json0 = _json.GetProperty("playerBornPos"); float __x; __x = _json0.GetProperty("x").GetSingle(); float __y; __y = _json0.GetProperty("y").GetSingle(); float __z; __z = _json0.GetProperty("z").GetSingle();  PlayerBornPos = new System.Numerics.Vector3(__x, __y,__z); }
        { var _json0 = _json.GetProperty("playerBornRote"); float __x; __x = _json0.GetProperty("x").GetSingle(); float __y; __y = _json0.GetProperty("y").GetSingle(); float __z; __z = _json0.GetProperty("z").GetSingle();  PlayerBornRote = new System.Numerics.Vector3(__x, __y,__z); }
        Exp = _json.GetProperty("exp").GetInt32();
        Coin = _json.GetProperty("coin").GetInt32();
        Crystal = _json.GetProperty("crystal").GetInt32();
        { var __json0 = _json.GetProperty("monsterList"); MonsterList = new System.Collections.Generic.List<MapMonster>(__json0.GetArrayLength()); foreach(JsonElement __e0 in __json0.EnumerateArray()) { MapMonster __v0;  __v0 = MapMonster.DeserializeMapMonster(__e0);  MonsterList.Add(__v0); }   }
        PostInit();
    }

    public MapConfig(int id, string mapName, string sceneName, int power, System.Numerics.Vector3 mainCamPos, System.Numerics.Vector3 mainCamRote, System.Numerics.Vector3 playerBornPos, System.Numerics.Vector3 playerBornRote, int exp, int coin, int crystal, System.Collections.Generic.List<MapMonster> monsterList ) 
    {
        this.Id = id;
        this.MapName = mapName;
        this.SceneName = sceneName;
        this.Power = power;
        this.MainCamPos = mainCamPos;
        this.MainCamRote = mainCamRote;
        this.PlayerBornPos = playerBornPos;
        this.PlayerBornRote = playerBornRote;
        this.Exp = exp;
        this.Coin = coin;
        this.Crystal = crystal;
        this.MonsterList = monsterList;
        PostInit();
    }

    public static MapConfig DeserializeMapConfig(JsonElement _json)
    {
        return new MapConfig(_json);
    }

    /// <summary>
    /// 地图ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 地图名
    /// </summary>
    public string MapName { get; private set; }
    /// <summary>
    /// 场景名
    /// </summary>
    public string SceneName { get; private set; }
    /// <summary>
    /// 需要体力
    /// </summary>
    public int Power { get; private set; }
    /// <summary>
    /// 初始相机位置
    /// </summary>
    public System.Numerics.Vector3 MainCamPos { get; private set; }
    /// <summary>
    /// 初始相机旋转
    /// </summary>
    public System.Numerics.Vector3 MainCamRote { get; private set; }
    /// <summary>
    /// 初始玩家位置
    /// </summary>
    public System.Numerics.Vector3 PlayerBornPos { get; private set; }
    /// <summary>
    /// 初始玩家旋转
    /// </summary>
    public System.Numerics.Vector3 PlayerBornRote { get; private set; }
    /// <summary>
    /// 经验值
    /// </summary>
    public int Exp { get; private set; }
    /// <summary>
    /// 金币
    /// </summary>
    public int Coin { get; private set; }
    /// <summary>
    /// 宝石
    /// </summary>
    public int Crystal { get; private set; }
    /// <summary>
    /// 怪物列表
    /// </summary>
    public System.Collections.Generic.List<MapMonster> MonsterList { get; private set; }

    public const int __ID__ = -1840922722;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var _e in MonsterList) { _e?.Resolve(_tables); }
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var _e in MonsterList) { _e?.TranslateText(translator); }
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "MapName:" + MapName + ","
        + "SceneName:" + SceneName + ","
        + "Power:" + Power + ","
        + "MainCamPos:" + MainCamPos + ","
        + "MainCamRote:" + MainCamRote + ","
        + "PlayerBornPos:" + PlayerBornPos + ","
        + "PlayerBornRote:" + PlayerBornRote + ","
        + "Exp:" + Exp + ","
        + "Coin:" + Coin + ","
        + "Crystal:" + Crystal + ","
        + "MonsterList:" + Bright.Common.StringUtil.CollectionToString(MonsterList) + ","
        + "}";
    }

    partial void PostInit();
    partial void PostResolve();
}
}