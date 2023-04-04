/****************************************************
	文件：IState.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月31日 星期四 12:10   	
	功能：状态接口
*****************************************************/

public interface IState {
    void Enter(EntityBase entity, params object[] args);
    void Process(EntityBase entity, params object[] args);
    void Exit(EntityBase entity, params object[] args);
}