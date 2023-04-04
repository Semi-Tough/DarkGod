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

public sealed partial class GuideConfig :  Bright.Config.BeanBase 
{
    public GuideConfig(JsonElement _json) 
    {
        Id = _json.GetProperty("id").GetInt32();
        NpcId = _json.GetProperty("npcId").GetInt32();
        DialogArr = _json.GetProperty("dialogArr").GetString();
        Type = (GuideType)_json.GetProperty("type").GetInt32();
        Coin = _json.GetProperty("coin").GetInt32();
        Exp = _json.GetProperty("exp").GetInt32();
        PostInit();
    }

    public GuideConfig(int id, int npcId, string dialogArr, GuideType type, int coin, int exp ) 
    {
        this.Id = id;
        this.NpcId = npcId;
        this.DialogArr = dialogArr;
        this.Type = type;
        this.Coin = coin;
        this.Exp = exp;
        PostInit();
    }

    public static GuideConfig DeserializeGuideConfig(JsonElement _json)
    {
        return new GuideConfig(_json);
    }

    /// <summary>
    /// 任务ID
    /// </summary>
    public int Id { get; private set; }
    public int NpcId { get; private set; }
    /// <summary>
    /// 任务对话
    /// </summary>
    public string DialogArr { get; private set; }
    /// <summary>
    /// 任务枚举
    /// </summary>
    public GuideType Type { get; private set; }
    /// <summary>
    /// 奖励金币
    /// </summary>
    public int Coin { get; private set; }
    /// <summary>
    /// 奖励经验
    /// </summary>
    public int Exp { get; private set; }

    public const int __ID__ = -1400953538;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "NpcId:" + NpcId + ","
        + "DialogArr:" + DialogArr + ","
        + "Type:" + Type + ","
        + "Coin:" + Coin + ","
        + "Exp:" + Exp + ","
        + "}";
    }

    partial void PostInit();
    partial void PostResolve();
}
}