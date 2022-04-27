/****************************************************
    文件：BaseData
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月05日 星期六 12:10
    功能：配置数据类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class BaseData
{
    public int id;
}

public class NameConfig : BaseData
{
    public string man;
    public string surname;
    public string woman;
}

public class MapConfig : BaseData
{
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public string mapName;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public int power;
    public string sceneName;
}

public class GuideConfig : BaseData
{
    public int actionId;
    public int coin;
    public string dialogArr;
    public int exp;
    public int npcId;
}

public class StrengthenConfig : BaseData
{
    public int addDefine;
    public int addHp;
    public int addHurt;
    public int coin;
    public int crystal;
    public int minLevel;
    public int position;
    public int starLevel;
}

public class TaskConfig : BaseData
{
    public string taskName;
    public int targetCount;
    public int exp;
    public int coin;
}

public class SkillConfig : BaseData
{
    public string skillName;
    public int skillTime;
    public int skillId;
    public string effectName;
    public List<int> skillMoveList;
}

public class SkillMoveConfig : BaseData
{
    public int delayTime; //毫秒
    public int moveTime; //毫秒
    public float moveDis;
}