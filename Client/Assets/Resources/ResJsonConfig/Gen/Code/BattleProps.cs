//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg
{ 

public sealed partial class BattleProps :  Bright.Config.BeanBase 
{
    public BattleProps(JSONNode _json) 
    {
        { if(!_json["hp"].IsNumber) { throw new SerializationException(); }  Hp = _json["hp"]; }
        { if(!_json["ad"].IsNumber) { throw new SerializationException(); }  Ad = _json["ad"]; }
        { if(!_json["ap"].IsNumber) { throw new SerializationException(); }  Ap = _json["ap"]; }
        { if(!_json["addef"].IsNumber) { throw new SerializationException(); }  Addef = _json["addef"]; }
        { if(!_json["apdef"].IsNumber) { throw new SerializationException(); }  Apdef = _json["apdef"]; }
        { if(!_json["dodge"].IsNumber) { throw new SerializationException(); }  Dodge = _json["dodge"]; }
        { if(!_json["pierce"].IsNumber) { throw new SerializationException(); }  Pierce = _json["pierce"]; }
        { if(!_json["critical"].IsNumber) { throw new SerializationException(); }  Critical = _json["critical"]; }
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

    public static BattleProps DeserializeBattleProps(JSONNode _json)
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