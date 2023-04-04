/****************************************************
	文件：SeverRoot.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：服务器初始化
*****************************************************/

using PETool.PELogger;
using Server;

namespace Sever {
	public class SeverRoot {
		private int sessionId;
		public static SeverRoot Instance { get; } = new SeverRoot();

		public void Init() {
			//数据库
			PELogger.InitSetting(new LogConfig{
				enableCover = true,
				enableLog = true,
				enableTime = true,
				enableThreadId = true,
				enableSave = true,
				enableTrace = true,
				loggerType = LoggerType.Console
			});
			// DataBaseManager.Instance.Init();

			//服务层
			DbSvc.Instance.Init();
			CacheSvc.Instance.Init();
			ResSvc.Instance.Init();
			TimerSvc.Instance.Init();
			NetSvc.Instance.Init();

			//业务逻辑层
			LoginSystem.Instance.Init();
			GuideSystem.Instance.Init();
			StrengthenSystem.Instance.Init();
			ChatSystem.Instance.Init();
			BuySystem.Instance.Init();
			PowerSystem.Instance.Init();
			TaskSystem.Instance.Init();
			DungeonSystem.Instance.Init();
		}

		public void UpData() {
			NetSvc.Instance.Updata();
		}

		public int GetSessionId() {
			if(sessionId == int.MaxValue) sessionId = 0;

			return sessionId += 1;
		}
	}
}