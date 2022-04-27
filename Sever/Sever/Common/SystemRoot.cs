/****************************************************
	文件：SystemRoot.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月26日 星期六 16:32   	
	功能：业务系统基类
*****************************************************/

using PEProtocol;

namespace Sever
{
    public class SystemRoot
    {
        protected CacheService CacheService;
        protected ConfigService ConfigService;
        protected TimerService TimerService;

        public virtual void Init()
        {
            ConfigService = ConfigService.Instance;
            CacheService = CacheService.Instance;
            TimerService = TimerService.Instance;
        }
    }
}