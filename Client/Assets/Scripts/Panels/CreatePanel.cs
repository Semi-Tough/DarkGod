/****************************************************
    文件：CreatePanel.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/23 19:46:33
    功能：角色创建界面
*****************************************************/

using cfg;
using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreatePanel : PanelRoot {
	public InputField InputName;

	override protected void InitPanel() {
		base.InitPanel();
		//显示一个随机名字
		InputName.text = GetRandomName();
	}

	// 点击随机名字按钮
	public void ClickRandomNameBtn() {
		audioSvc.Play2DAudio(Constants.UIClickBtn);
		InputName.text = GetRandomName();
	}

	// 点击进入游戏按钮
	public void ClickEnterBtn() {
		audioSvc.Play2DAudio(Constants.UILoginBtn);
		if(InputName.text != "") {
			//发送名字数据到服务器,登陆主城
			NetMsg msg = new NetMsg{
				Head = new Head{
					Cmd = Cmd.CmdRequestRename,
				},
				Body = new Body(){
					RequestRename = new RequestRename{
						Name = InputName.text
					}
				}
			};
			netSvc.SendMessage(msg);
		}
		else {
			GameRoot.AddTips("该名称不合法...");
		}
	}
	public string GetRandomName() {
		Dictionary<int, NameConfig> nameDic = resSvc.GetNameConfig();
		int length = nameDic.Count;
		bool man = Random.Range(0, 2) == 1;
		string randomName =
			nameDic[Random.Range(0, length)].Surname;
		if(man)
			randomName += nameDic[Random.Range(0, length)].Man;
		else
			randomName +=
				nameDic[Random.Range(0, length)].Woman;
		return randomName;
	}
}