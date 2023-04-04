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

public sealed partial class NameConfig :  Bright.Config.BeanBase 
{
    public NameConfig(JsonElement _json) 
    {
        Id = _json.GetProperty("id").GetInt32();
        Surname = _json.GetProperty("surname").GetString();
        Man = _json.GetProperty("man").GetString();
        Woman = _json.GetProperty("woman").GetString();
        PostInit();
    }

    public NameConfig(int id, string surname, string man, string woman ) 
    {
        this.Id = id;
        this.Surname = surname;
        this.Man = man;
        this.Woman = woman;
        PostInit();
    }

    public static NameConfig DeserializeNameConfig(JsonElement _json)
    {
        return new NameConfig(_json);
    }

    public int Id { get; private set; }
    /// <summary>
    /// 姓
    /// </summary>
    public string Surname { get; private set; }
    /// <summary>
    /// 男名
    /// </summary>
    public string Man { get; private set; }
    /// <summary>
    /// 女名
    /// </summary>
    public string Woman { get; private set; }

    public const int __ID__ = 782791117;
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
        + "Surname:" + Surname + ","
        + "Man:" + Man + ","
        + "Woman:" + Woman + ","
        + "}";
    }

    partial void PostInit();
    partial void PostResolve();
}
}
