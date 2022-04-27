/****************************************************
	文件：BattleSystem.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 14:20   	
	功能：战斗业务系统
*****************************************************/

using PEProtocol;
using UnityEngine;

public class BattleSystem : SystemRoot
{
    public static BattleSystem Instance { get; private set; }
    public BattleWindow battleWindow;
    public BattleManager battleManager;

    public override void InitSystem()
    {
        base.InitSystem();
        Instance = this;
        PeCommon.Log("战斗系统加载完成");
    }

    public void StartBattle(int mapID)
    {
        GameObject go = new GameObject("BattleRoot");
        go.transform.SetParent(GameRoot.instance.transform);
        battleManager = go.AddComponent<BattleManager>();
        battleManager.Init(mapID);
        SetBattleWindowState();
    }

    public void SetBattleWindowState(bool state = true)
    {
        battleWindow.SetWindowState(state);
    }

    public void SetPlayerMove(Vector2 dir)
    {
        battleManager.SetPlayerMove(dir);
    }


    public void SetPlayerAttack(int index)
    {
        battleManager.SetPlayerAttack(index);
    }

    public Vector2 GetInPutDic()
    {
        return battleWindow.currentDir;
    }
}