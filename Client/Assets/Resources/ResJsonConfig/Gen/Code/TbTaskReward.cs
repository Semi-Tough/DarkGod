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

public sealed partial class TbTaskReward
{
    private readonly Dictionary<int, TaskRewardConfig> _dataMap;
    private readonly List<TaskRewardConfig> _dataList;
    
    public TbTaskReward(JSONNode _json)
    {
        _dataMap = new Dictionary<int, TaskRewardConfig>();
        _dataList = new List<TaskRewardConfig>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = TaskRewardConfig.DeserializeTaskRewardConfig(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, TaskRewardConfig> DataMap => _dataMap;
    public List<TaskRewardConfig> DataList => _dataList;

    public TaskRewardConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public TaskRewardConfig Get(int key) => _dataMap[key];
    public TaskRewardConfig this[int key] => _dataMap[key];

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