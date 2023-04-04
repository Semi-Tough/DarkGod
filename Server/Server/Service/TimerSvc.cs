/****************************************************
	文件：TimerService.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月23日 星期三 18:43   	
	功能：定时服务
*****************************************************/

using PETool.PELogger;
using PETool.PETimer;

namespace Sever {
	public class TimerSvc {
		public static TimerSvc Instance { get; } = new TimerSvc();

		private AsyncTimer? asyncTimer;
		public void Init() {
			asyncTimer = new AsyncTimer(false){
				logFunc = PELogger.Log,
				wainFunc = PELogger.Wain,
				errorFunc = PELogger.Wain
			};

			PELogger.Log("TimerService Init Done.");
		}


		public int AddTimeTask(uint delay, Action<int> taskCb, Action<int>? cancelCb, int count = 1) {
			if(asyncTimer != null) {
				return asyncTimer.AddTask(delay, taskCb, cancelCb, count);
			}
			else {
				return-1;
			}
		}

		public double GetUtcMilliseconds() {
			if(asyncTimer != null) {
				return asyncTimer.GetUtcMilliseconds();
			}
			else {
				return-1;
			}
		}
	}
}