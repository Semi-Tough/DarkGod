/****************************************************
    文件：GuidePanel.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/3/9 15:19:23
    功能：引导对话界面
*****************************************************/

using cfg;
using PEProtocol;
using UnityEngine.UI;
public class GuidePanel : PanelRoot {
	public Text TxtTalk;
	public Text TxtName;
	public Image ImgIcon;

	private GuideConfig currentGuideDate;
	private string[] dialogArr;
	private int index;

	override protected void InitPanel() {
		base.InitPanel();
		currentGuideDate = MainCitySys.Instance.GetGuideConfig();
		dialogArr = currentGuideDate.DialogArr.Split('#');
		index = 1;
		SetTalk();
	}

	private void SetTalk() {
		string[] talkArr = dialogArr[index].Split('|');
		if(talkArr[0] == "0") {
			//玩家
			SetSprite(ImgIcon, PathDefine.PlayerIcon);
			SetText(TxtName, playerData.Name);
		}
		else {
			//NPC
			switch(currentGuideDate.NpcId) {
				case Constants.NpcWiseMan:
					SetSprite(ImgIcon, PathDefine.WiseIcon);
					SetText(TxtName, "智者");
					break;
				case Constants.NpcGeneralMan:
					SetSprite(ImgIcon, PathDefine.GeneralIcon);
					SetText(TxtName, "将军");
					break;
				case Constants.NpcArtisanMan:
					SetSprite(ImgIcon, PathDefine.ArtisanIcon);
					SetText(TxtName, "工匠");
					break;
				case Constants.NpcTraderMan:
					SetSprite(ImgIcon, PathDefine.TraderIcon);
					SetText(TxtName, "商人");
					break;
				default:
					SetSprite(ImgIcon, PathDefine.GuideIcon);
					SetText(TxtName, "引导者");
					break;
			}
		}

		SetText(TxtTalk, talkArr[1].Contains("$name") ? talkArr[1].Replace("$name", playerData.Name) : talkArr[1]);

		ImgIcon.SetNativeSize();
	}

	#region 点击事件
	public void ClickNextBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		index += 1;
		if(index == dialogArr.Length) {
			//发送引导任务完成信息
			NetMsg msg = new NetMsg{
				Head = new Head(){
					Cmd = Cmd.CmdRequestGuide,
				},
				Body = new Body(){
					RequestGuide = new RequestGuide{
						GuideId = currentGuideDate.Id
					}
				}
			};

			netSvc.SendMessage(msg);
			SetPanelState(false);
		}
		else {
			SetTalk();
		}
	}
	#endregion
}