/****************************************************
	文件：DungeonSystem.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月27日 星期日 14:53   	
	功能：副本业务系统
*****************************************************/

using PEProtocol;
using PETool.PELogger;
public class DungeonSys : SystemRoot {
	public static DungeonSys Instance { get; private set; }
	public DungeonPanel DungeonPanel;

	public override void InitSystem() {
		base.InitSystem();
		Instance = this;
		PELogger.Log("副本系统加载完成");
	}

	public void EnterDungeonPanel() {
		SetDungeonPanel();
	}

	private void SetDungeonPanel(bool state = true) {
		DungeonPanel.SetPanelState(state);
	}

	public void ResponseDungeon(NetMsg message) {
		gameRoot.SetPlayerPowerByDungeon(message.Body.ResponseDungeon);
		MainCitySys.Instance.MainCityPanel.SetPanelState(false);
		SetDungeonPanel(false);

		BattleSys.Instance.StartBattle(message.Body.ResponseDungeon.DungeonId);
	}
}