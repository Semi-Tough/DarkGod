/****************************************************
	文件：TaskItem.cs
	作者：Semi-Tough
	邮箱: 1693416984@qq.com
	日期：2022年03月26日 星期六 14:55   	
	功能：存储当前任务的信息
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
public class TaskItem : MonoBehaviour
{
    public Text TxtName;
    public Text TxtProgress;
    public Text TxtCoin;
    public Text TxtExp;

    public Image ImgFill;
    public Image ImgFinished;
    public Button BtnReward;
}

// public class TaskData {
// 	public int id;
//     public int progress;
//     public bool finished;
// }