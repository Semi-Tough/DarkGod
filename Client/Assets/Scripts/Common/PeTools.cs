/****************************************************
    文件：PETools.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/24 13:39:2
    功能：工具类
*****************************************************/

using System;

public static class PeTools
{
    public static int RandomInt(int min, int max, Random random = null)
    {
        random ??= new Random();
        int value = random.Next(min, max + 1);
        return value;
    }
}