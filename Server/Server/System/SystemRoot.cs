/****************************************************
	文件：SystemRoot.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月26日 星期六 16:32   	
	功能：业务系统基类
*****************************************************/

namespace Sever {
	public class SystemRoot<T> where T : new() {
		public static T Instance { get; } = new T();

		protected CacheSvc cacheSvc = null!;
		protected ResSvc resSvc = null!;
		protected TimerSvc timerSvc = null!;

		public virtual void Init() {
			resSvc = ResSvc.Instance;
			cacheSvc = CacheSvc.Instance;
			timerSvc = TimerSvc.Instance;
		}
	}
}