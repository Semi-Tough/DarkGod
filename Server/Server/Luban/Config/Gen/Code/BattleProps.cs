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

public sealed partial class BattleProps :  Bright.Config.BeanBase 
{
    public BattleProps(JsonElement _json) 
    {
        Hp = _json.GetProperty("hp").GetInt32();
        Ad = _json.GetProperty("ad").GetInt32();
        Ap = _json.GetProperty("ap").GetInt32();
        Addef = _json.GetProperty("addef").GetInt32();
        Apdef = _json.GetProperty("apdef").GetInt32();
        Dodge = _json.GetProperty("dodge").GetInt32();
        Pierce = _json.GetProperty("pierce").GetInt32();
        Critical = _json.GetProperty("critical").GetInt32();
        PostInit();
    }

    public BattleProps(int hp, int ad, int ap, int addef, int apdef, int dodge, int pierce, int critical ) 
    {
        this.Hp = hp;
        this.Ad = ad;
        this.Ap = ap;
        this.Addef = addef;
        this.Apdef = apdef;
        this.Dodge = dodge;
        this.Pierce = pierce;
        this.Critical = critical;
        PostInit();
    }

    public static BattleProps DeserializeBattleProps(JsonElement _json)
    {
        return new BattleProps(_json);
    }

    /// <summary>
    /// 血量
    /// </summary>
    public int Hp { get; private set; }
    /// <summary>
    /// 物理攻击
    /// </summary>
    public int Ad { get; private set; }
    /// <summary>
    /// 法术攻击
    /// </summary>
    public int Ap { get; private set; }
    /// <summary>
    /// 物理防御
    /// </summary>
    public int Addef { get; private set; }
    /// <summary>
    /// 法术防御
    /// </summary>
    public int Apdef { get; private set; }
    /// <summary>
    /// 闪避
    /// </summary>
    public int Dodge { get; private set; }
    /// <summary>
    /// 穿透
    /// </summary>
    public int Pierce { get; private set; }
    /// <summary>
    /// 暴击
    /// </summary>
    public int Critical { get; private set; }

    public const int __ID__ = 904651640;
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
        + "Hp:" + Hp + ","
        + "Ad:" + Ad + ","
        + "Ap:" + Ap + ","
        + "Addef:" + Addef + ","
        + "Apdef:" + Apdef + ","
        + "Dodge:" + Dodge + ","
        + "Pierce:" + Pierce + ","
        + "Critical:" + Critical + ","
        + "}";
    }

    partial void PostInit();
    partial void PostResolve();
}
}
