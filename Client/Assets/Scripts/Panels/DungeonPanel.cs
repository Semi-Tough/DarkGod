/****************************************************
	文件：DungeonPanel.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月27日 星期日 14:35   	
	功能：副本选择界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
public class DungeonPanel : PanelRoot {
	public Transform PointerTransform;
	public Button[] BtnDungeons;

	override protected void InitPanel() {
		base.InitPanel();
		RefreshUI();
	}

	private void RefreshUI() {
		int dungeonId = playerData.DungeonId;
		for(int i = 0; i < BtnDungeons.Length; i++) {
			if(i < dungeonId % 10000) {
				SetActive(BtnDungeons[i].gameObject);
				if(i != dungeonId % 10000 - 1) continue;
				PointerTransform.SetParent(BtnDungeons[i].transform);
				PointerTransform.localPosition = new Vector3(60, 90, 0);
			}
			else {
				SetActive(BtnDungeons[i].gameObject, false);
			}
		}
	}


	#region 点击事件
	public void ClickDungeonBtn(int dungeonId) {
		audioSvc.Play2DAudio(Constants.UIClickBtn);

		//检查玩家体力是否足够
		if(playerData.Power > resSvc.GetMapConfig(dungeonId).Power) {
			//副本请求
			netSvc.SendMessage(new NetMsg{
				Head = new Head(){
					Cmd = Cmd.CmdRequestDungeon,
				},
				Body = new Body(){
					RequestDungeon = new RequestDungeon{
						DungeonId = dungeonId
					}
				}
			});
		}
		else {
			GameRoot.AddTips("剩余体力值不足..");
		}
	}

	public void ClickCloseBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		SetPanelState(false);
	}
	#endregion
}