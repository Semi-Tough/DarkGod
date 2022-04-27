/****************************************************
	文件：DungeonSystem.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月27日 星期日 14:53   	
	功能：副本业务系统
*****************************************************/

using PEProtocol;

public class DungeonSystem : SystemRoot
{
    public static DungeonSystem Instance { get; private set; }
    public DungeonWindow dungeonWindow;

    public override void InitSystem()
    {
        base.InitSystem();
        Instance = this;
        PeCommon.Log("副本系统加载完成");
    }

    public void EnterDungeonWindow()
    {
        SetDungeonWindow();
    }

    private void SetDungeonWindow(bool state = true)
    {
        dungeonWindow.SetWindowState(state);
    }

    public void ResponseDungeon(GameMessage message)
    {
        gameRoot.SetPlayerDataByDungeonPush(message.ResponseDungeon);
        MainCitySystem.Instance.mainCityWindow.SetWindowState(false);
        SetDungeonWindow(false);

        BattleSystem.Instance.StartBattle(message.ResponseDungeon.DungeonId);
    }
}