/****************************************************
    文件：LoginWindow.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/22 19:23:15
    功能：登陆注册界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : WindowRoot
{
    public InputField iptAccount;
    public InputField iptPassword;
    public Button btnEnter;
    public Button btnNotice;

    protected override void InitWindow()
    {
        base.InitWindow();

        if (PlayerPrefs.HasKey("Account") && PlayerPrefs.HasKey("Password"))
        {
            iptAccount.text = PlayerPrefs.GetString("Account");
            iptPassword.text = PlayerPrefs.GetString("Password");
        }
        else
        {
            iptAccount.text = "";
            iptPassword.text = "";
        }
    }


    #region 点击事件

    public void ClickEnterBtn()
    {
        audioService.PlayUiAudio(Constants.UiLoginBtn);
        string account = iptAccount.text;
        string password = iptPassword.text;
        if (account != "" && password != "")
        {
            //更新本地存储的账号
            PlayerPrefs.SetString("Account", account);
            PlayerPrefs.SetString("Password", password);

            //发送网络消息,请求登陆
            GameMessage msg = new GameMessage
            {
                cmd = (int) Cmd.RequestLogin,
                RequestLogin = new RequestLogin
                {
                    Account = account,
                    Password = password
                }
            };
            netService.SendMessage(msg);
        }
        else
        {
            GameRoot.AddTips("账户或密码不能为空...");
        }
    }

    public void ClickNoticeBtn()
    {
        GameRoot.AddTips("功能正在开发中...");
        audioService.PlayUiAudio(Constants.UiClickBtn);
    }

    #endregion
}