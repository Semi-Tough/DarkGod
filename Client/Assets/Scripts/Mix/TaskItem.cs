/****************************************************
	文件：TaskItem.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月26日 星期六 14:55   	
	功能：存储当前任务的信息
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class TaskItem : MonoBehaviour
{
    public Text txtName;
    public Text txtProgress;
    public Text txtCoin;
    public Text txtExp;

    public Image imgFill;
    public Image imgFinished;
    public Button btnReward;
}

public class TaskData : BaseData
{
    public int progress;
    public bool finished;
}