/****************************************************
	文件：SeverRoot.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：服务器初始化
*****************************************************/

namespace Sever
{
    public class SeverRoot
    {
        private static SeverRoot _instance;

        private int _sessionId;
        public static SeverRoot Instance => _instance ?? (_instance = new SeverRoot());

        public void Init()
        {
            //数据库
            DataBaseManager.Instance.Init();

            //服务层
            CacheService.Instance.Init();
            ConfigService.Instance.Init();
            TimerService.Instance.Init();
            NetService.Instance.Init();

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

        public void UpData()
        {
            NetService.Instance.UpData();
            TimerService.Instance.Update();
        }

        public int GetSessionId()
        {
            if (_sessionId == int.MaxValue) _sessionId = 0;

            return _sessionId += 1;
        }
    }
}