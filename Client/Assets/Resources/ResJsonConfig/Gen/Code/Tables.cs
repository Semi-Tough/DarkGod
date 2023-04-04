//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;


namespace cfg
{ 
   
public sealed partial class Tables
{
    public TbGuide TbGuide {get; }
    public TbMap TbMap {get; }
    public TbMonster TbMonster {get; }
    public TbName TbName {get; }
    public TbSkill TbSkill {get; }
    public TbSkillMove TbSkillMove {get; }
    public TbSkillRange TbSkillRange {get; }
    public TbStrengthen TbStrengthen {get; }
    public TbTaskReward TbTaskReward {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbGuide = new TbGuide(loader("tbguide")); 
        tables.Add("TbGuide", TbGuide);
        TbMap = new TbMap(loader("tbmap")); 
        tables.Add("TbMap", TbMap);
        TbMonster = new TbMonster(loader("tbmonster")); 
        tables.Add("TbMonster", TbMonster);
        TbName = new TbName(loader("tbname")); 
        tables.Add("TbName", TbName);
        TbSkill = new TbSkill(loader("tbskill")); 
        tables.Add("TbSkill", TbSkill);
        TbSkillMove = new TbSkillMove(loader("tbskillmove")); 
        tables.Add("TbSkillMove", TbSkillMove);
        TbSkillRange = new TbSkillRange(loader("tbskillrange")); 
        tables.Add("TbSkillRange", TbSkillRange);
        TbStrengthen = new TbStrengthen(loader("tbstrengthen")); 
        tables.Add("TbStrengthen", TbStrengthen);
        TbTaskReward = new TbTaskReward(loader("tbtaskreward")); 
        tables.Add("TbTaskReward", TbTaskReward);
        PostInit();

        TbGuide.Resolve(tables); 
        TbMap.Resolve(tables); 
        TbMonster.Resolve(tables); 
        TbName.Resolve(tables); 
        TbSkill.Resolve(tables); 
        TbSkillMove.Resolve(tables); 
        TbSkillRange.Resolve(tables); 
        TbStrengthen.Resolve(tables); 
        TbTaskReward.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbGuide.TranslateText(translator); 
        TbMap.TranslateText(translator); 
        TbMonster.TranslateText(translator); 
        TbName.TranslateText(translator); 
        TbSkill.TranslateText(translator); 
        TbSkillMove.TranslateText(translator); 
        TbSkillRange.TranslateText(translator); 
        TbStrengthen.TranslateText(translator); 
        TbTaskReward.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}