/****************************************************
    文件：LoginSystem.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/2/22 12:45:0
    功能：登陆注册业务系统
*****************************************************/

using PEProtocol;
using PETool.PELogger;
public class LoginSys : SystemRoot {
	public static LoginSys Instance { get; private set; }
	public LoginPanel LoginPanel;
	public CreatePanel CreatePanel;

	public override void InitSystem() {
		base.InitSystem();
		Instance = this;
		PELogger.Log("登陆系统加载完成");
	}

	//进入登陆场景
	public void LoadingLoginPanel() {
		PELogger.Log("进入加载界面");
		//异步的加载登陆场景, 显示加载的进度条,打开注册登陆界面
		resSvc.AsyncLoadScene(Constants.SceneLogin, () => {
			PELogger.Log("登陆场景加载完成");
			LoginPanel.SetPanelState();
			audioSvc.PlayBgMusic(Constants.BgLogin);
		});
	}

	public void ResponseLogin(NetMsg message) {
		GameRoot.Instance.SetPlayerData(message.Body.ResponseLogin);

		if(message.Body.ResponseLogin.PlayerData.Name == "") //打开角色创建界面
		{
			CreatePanel.SetPanelState();
			PELogger.Log("进入角色创建界面");
		}
		else //进入主城
		{
			MainCitySys.Instance.LoadingMainCitySystem();
		}

		GameRoot.AddTips("登陆成功...");
		//关闭登陆界面
		LoginPanel.SetPanelState(false);
	}

	public void ResponseRename(NetMsg message) {
		GameRoot.Instance.SetPlayerName(message.Body.ResponseRename.Name);
		//关闭创建界面
		CreatePanel.SetPanelState(false);
		//进入主城
		MainCitySys.Instance.LoadingMainCitySystem();
	}
}