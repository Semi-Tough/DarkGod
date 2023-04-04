/****************************************************
    文件：LoginPanel.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/22 19:23:15
    功能：登陆注册界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
public class LoginPanel : PanelRoot {
	public InputField AccountField;
	public InputField PasswordField;
	public Button BtnEnter;
	public Button BtnNotice;

	override protected void InitPanel() {
		base.InitPanel();
		if(PlayerPrefs.HasKey("Account") && PlayerPrefs.HasKey("Password")) {
			AccountField.text = PlayerPrefs.GetString("Account");
			PasswordField.text = PlayerPrefs.GetString("Password");
		}
		else {
			AccountField.text = "";
			PasswordField.text = "";
		}
	}


	#region 点击事件
	public void ClickEnterBtn() {
		audioSvc.Play2DAudio(Constants.UILoginBtn);
		string account = AccountField.text;
		string password = PasswordField.text;
		if(account != "" && password != "") {
			//更新本地存储的账号
			PlayerPrefs.SetString("Account", account);
			PlayerPrefs.SetString("Password", password);

			//发送网络消息,请求登陆
			NetMsg msg = new NetMsg{
				Head = new Head(){
					Cmd = Cmd.CmdRequestLogin,
				},
				Body = new Body(){
					RequestLogin = new RequestLogin{
						Account = account,
						Password = password
					}
				}
			};
			netSvc.SendMessage(msg);
		}
		else {
			GameRoot.AddTips("账户或密码不能为空...");
		}
	}

	public void ClickNoticeBtn() {
		GameRoot.AddTips("功能正在开发中...");
		audioSvc.Play2DAudio(Constants.UIClickBtn);
	}
	#endregion
}