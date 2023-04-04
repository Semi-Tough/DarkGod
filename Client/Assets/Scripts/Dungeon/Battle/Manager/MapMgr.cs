/****************************************************
    文件：MapManager.cs
    作者：Semi-Tough
    邮箱: 1693416984@qq.com
    日期：2022年03月30日 星期三 15:02       
    功能：地图管理器
*****************************************************/

using PETool.PELogger;
using UnityEngine;
public class MapMgr : MonoBehaviour {
	public int waveIndex = 1;
	public Door[] doorArr;

	private BattleMgr battleMgr;

	public void Init(BattleMgr battleMgr) {
		this.battleMgr = battleMgr;
		LoadMonsterByWaveId(waveIndex);
		PELogger.Log("地图管理器加载完成");
	}
	public bool OpenNextDoor() {
		waveIndex += 1;
		foreach(Door door in doorArr) {
			if(door.waveIndex == waveIndex) {
				door.GetComponent<BoxCollider>().isTrigger = true;
				return true;
			}
		}
		return false;
	}
	public void LoadMonsterByWaveId(int waveIndex) {
		battleMgr.LoadMonsterByWaveId(waveIndex);
		battleMgr.ActiveCurrentWaveMonster();
		battleMgr.nextDoor = true;
	}
}