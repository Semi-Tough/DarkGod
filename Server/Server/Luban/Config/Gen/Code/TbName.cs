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


public sealed partial class TbName
{
    private readonly Dictionary<int, NameConfig> _dataMap;
    private readonly List<NameConfig> _dataList;
    
    public TbName(JsonElement _json)
    {
        _dataMap = new Dictionary<int, NameConfig>();
        _dataList = new List<NameConfig>();
        
        foreach(JsonElement _row in _json.EnumerateArray())
        {
            var _v = NameConfig.DeserializeNameConfig(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, NameConfig> DataMap => _dataMap;
    public List<NameConfig> DataList => _dataList;

    public NameConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public NameConfig Get(int key) => _dataMap[key];
    public NameConfig this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    

    partial void PostInit();
    partial void PostResolve();
}

}