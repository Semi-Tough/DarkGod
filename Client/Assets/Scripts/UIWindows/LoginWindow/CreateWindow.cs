/****************************************************
    文件：CreateWindow.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/23 19:46:33
    功能：角色创建界面
*****************************************************/

using PEProtocol;
using UnityEngine.UI;

public class CreateWindow : WindowRoot
{
    public InputField inputName;

    protected override void InitWindow()
    {
        base.InitWindow();
        //显示一个随机名字
        inputName.text = resourceService.GetRandomNameConfig(false);
    }

    // 点击随机名字按钮
    public void ClickRandomNameBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        inputName.text = resourceService.GetRandomNameConfig(false);
    }

    // 点击进入游戏按钮
    public void ClickEnterBtn()
    {
        audioService.PlayUiAudio(Constants.UiLoginBtn);
        if (inputName.text != "")
        {
            //发送名字数据到服务器,登陆主城
            GameMessage msg = new GameMessage
            {
                cmd = (int) Cmd.RequestRename,
                RequestRename = new RequestRename
                {
                    Name = inputName.text
                }
            };
            netService.SendMessage(msg);
        }
        else
        {
            GameRoot.AddTips("该名称不合法...");
        }
    }
}