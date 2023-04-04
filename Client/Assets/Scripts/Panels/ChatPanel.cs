/****************************************************
    文件：ChatPanel
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年03月14日 星期一 11:53
    功能：聊天界面
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatPanel : PanelRoot {
	public Scrollbar ChatScrollbar;
	public InputField InputChat;
	public Button BtnSend;
	public Text TxtChat;
	public Image ImgFriend;
	public Image ImgGuild;
	public Image ImgWord;

	public Transform BtnChatGroup;

	private bool canSend = true;
	private ChatType currentType;

	private enum ChatType {
		Word,
		Guild,
		Friend
	}

	private readonly List<string> chatList = new List<string>();


	override protected void InitPanel() {
		base.InitPanel();
		RegisterClickEvents();
		currentType = ChatType.Word;
		ClickBtnChat(currentType);
	}

	private void RegisterClickEvents() {
		for(int i = 0; i < BtnChatGroup.childCount; i++) {
			Image image = BtnChatGroup.GetChild(i).GetComponent<Image>();
			switch(i) {
				case 0:
					ImgWord = image;
					break;
				case 1:
					ImgGuild = image;
					break;
				case 2:
					ImgFriend = image;
					break;
			}

			OnClick(image.gameObject, obj => {
				audioSvc.Play2DAudio(Constants.UIClickBtn);
				currentType = (ChatType)obj;
				ClickBtnChat(currentType);
			}, i);
		}
	}

	private void ClickBtnChat(ChatType chatType) {
		switch(chatType) {
			case ChatType.Word: {
				string chatMessage = "";
				foreach(string chat in chatList) chatMessage += $"{chat}\n";
				SetText(TxtChat, chatMessage);
				InputChat.interactable = true;
				BtnSend.interactable = true;
				ChatScrollbar.value = 0;
				SetSprite(ImgWord, PathDefine.ChatSelectedBtnImg);
				SetSprite(ImgGuild, PathDefine.ChatCommonBtnImg);
				SetSprite(ImgFriend, PathDefine.ChatCommonBtnImg);
			}
				break;
			case ChatType.Guild: {
				SetText(TxtChat, "尚未加入公会...");
				InputChat.interactable = false;
				BtnSend.interactable = false;
				SetSprite(ImgWord, PathDefine.ChatCommonBtnImg);
				SetSprite(ImgGuild, PathDefine.ChatSelectedBtnImg);
				SetSprite(ImgFriend, PathDefine.ChatCommonBtnImg);
				break;
			}
			case ChatType.Friend: {
				SetText(TxtChat, "暂无好友信息...");
				InputChat.interactable = false;
				BtnSend.interactable = false;
				SetSprite(ImgWord, PathDefine.ChatCommonBtnImg);
				SetSprite(ImgGuild, PathDefine.ChatCommonBtnImg);
				SetSprite(ImgFriend, PathDefine.ChatSelectedBtnImg);
			}
				break;
		}
	}


	public void AddMessage(string playerName, string chat) {
		chatList.Add(Constants.Color(playerName + "：", TextColor.Blue) + chat);
		if(!GetPanelState()) return;
		ClickBtnChat(currentType);
	}


	#region 点击事件
	public void ClickSendBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		if(!canSend) {
			GameRoot.AddTips("消息发送过于频繁请稍后重试...");
			return;
		}

		if(!string.IsNullOrWhiteSpace(InputChat.text)) {
			if(InputChat.text!.Length > 20) {
				GameRoot.AddTips("发送聊天消息不能超过20个字符...");
			}
			else {
				//发送网络消息到服务器
				NetMsg message = new NetMsg{
					Head = new Head{
						Cmd = Cmd.CmdSendChat,
					},
					Body = new Body{
						SendChat = new SendChat{
							Chat = InputChat.text
						}
					}
				};
				netSvc.SendMessage(message);
				InputChat.text = "";
				canSend = false;
				timerSvc.AddTask(1000, tid => {
					canSend = true;
				});
			}
		}
		else {
			GameRoot.AddTips("请输入聊天信息...");
		}
	}
	public void ClickCloseBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		SetPanelState(false);
	}
	#endregion
}