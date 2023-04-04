/****************************************************
    文件：StopTipsAnimation.cs
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022/3/14 10:37:20
    功能：关闭提示动画
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
public class StopTipsAnimation : MonoBehaviour
{
    public Text TxtTips;

    public void StopAnimation()
    {
        TxtTips.gameObject.SetActive(false);
    }
}