/****************************************************
	文件：SeverStart.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：服务器入口
*****************************************************/

namespace Sever
{
    internal static class SeverStart
    {
        private static void Main()
        {
            SeverRoot.Instance.Init();

            while (true)
            {
                SeverRoot.Instance.UpData();
                Thread.Sleep(20);
            }
        }
    }
}