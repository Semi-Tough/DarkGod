/****************************************************
    文件：LoginSystem.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/22 12:45:0
    功能：登陆注册业务系统
*****************************************************/

using PEProtocol;

public class LoginSystem : SystemRoot
{
    public static LoginSystem Instance { get; private set; }
    public LoginWindow loginWindow;
    public CreateWindow createWindow;

    public override void InitSystem()
    {
        base.InitSystem();
        Instance = this;
        PeCommon.Log("登陆系统加载完成");
    }

    //进入登陆场景
    public void LoadingLoginWindow()
    {
        PeCommon.Log("进入加载界面");
        //异步的加载登陆场景, 显示加载的进度条,打开注册登陆界面
        StartCoroutine(resourceService.AsyncLoadScene(Constants.SceneLogin, () =>
        {
            PeCommon.Log("登陆场景加载完成");
            loginWindow.SetWindowState();
            audioService.PlayBgMusic(Constants.BgLogin);
        }));
    }

    public void ResponseLogin(GameMessage message)
    {
        GameRoot.instance.SetPlayerDataByLogin(message.ResponseLogin);

        if (message.ResponseLogin.PlayerData.Name == "") //打开角色创建界面
        {
            createWindow.SetWindowState();
            PeCommon.Log("进入角色创建界面");
        }
        else //进入主城
        {
            MainCitySystem.Instance.LoadingMainCitySystem();
        }

        GameRoot.AddTips("登陆成功...");
        //关闭登陆界面
        loginWindow.SetWindowState(false);
    }

    public void ResponseRename(GameMessage message)
    {
        GameRoot.instance.SetPlayerDataByName(message.ResponseRename.Name);
        //关闭创建界面
        createWindow.SetWindowState(false);
        //进入主城
        MainCitySystem.Instance.LoadingMainCitySystem();
    }
}