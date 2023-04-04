/****************************************************
	文件：BattleSystem.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 14:20   	
	功能：战斗业务系统
*****************************************************/

using PEProtocol;
using PETool.PELogger;
using UnityEngine;
public class BattleSys : SystemRoot {
	public static BattleSys Instance { get; private set; }
	public BattleMgr BattleMgr;
	public BattlePanel BattlePanel;
	public EndPanel endPanel;
	private int mapId;
	private double startTime;
	public override void InitSystem() {
		base.InitSystem();
		Instance = this;
		Physics.IgnoreLayerCollision(8, 8, true);
		PELogger.Log("战斗系统加载完成");
	}
	public void StartBattle(int mapID) {
		mapId = mapID;
		GameObject go = new GameObject("BattleRoot");
		go.transform.SetParent(GameRoot.Instance.transform);
		BattleMgr = go.AddComponent<BattleMgr>();
		BattleMgr.Init(mapID, () => {
			startTime = timerSvc.GetNowTime();
		});
		SetBattlePanelState();
	}
	public void EndBattle(bool isWin, int restHp) {
		if(isWin) {
			double endTime = timerSvc.GetNowTime();
			NetMsg message = new NetMsg{
				Head = new Head(){
					Cmd = Cmd.CmdRequestEndDungeon,
				},
				Body = new Body(){
					RequestEndDungeon = new RequestEndDungeon{
						CostTime = (int)((endTime - startTime) / 1000),
						DungeonId = mapId,
						RestHp = restHp,
						Win = true
					}
				}
			};
			netSvc.SendMessage(message);
		}
		else {
			SetEndPanelState(EndType.Defeat);
		}
	}
	public void SetBattlePanelState(bool state = true) {
		BattlePanel.SetPanelState(state);
	}
	public void SetEndPanelState(EndType endType, bool state = true) {
		endPanel.SetEndType(endType);
		endPanel.SetPanelState(state);
	}
	public void SetPlayerMove(Vector2 dir) {
		BattleMgr.SetPlayerMove(dir);
	}
	public void SetPlayerAttack(int index) {
		BattleMgr.SetPlayerAttack(index);
	}
	public void EndBattleSystem() {
		BattlePanel.RemoveMonsterHUDItemAll();
		BattlePanel.SetPanelState(false);
		Destroy(BattleMgr.gameObject);
	}
	public void ResponseEndDungeon(NetMsg message) {
		ResponseEndDungeon data = message.Body.ResponseEndDungeon;
		gameRoot.SetPlayerDataByEndDungeon(data);
		SetBattlePanelState(false);
		SetEndPanelState(EndType.Victory);
		endPanel.SetVictoryData(data.DungeonId, data.CostTime, data.RestHp);
	}
	public Vector2 GetMoveDir() {
		return BattlePanel.MoveDir;
	}
}