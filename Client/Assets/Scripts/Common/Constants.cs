/****************************************************
    文件：Constants.cs
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022/2/22 14:13:21
    功能：常量配置
*****************************************************/

public enum TextColor
{
    Red,
    Green,
    Blue,
    Yellow
}

public static class Constants
{
    //颜色
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";


    //场景名称/ID
    public const string SceneLogin = "SceneLogin";
    public const int MainCityId = 10000;

    //音效名称
    public const string BgLogin = "bgLogin";
    public const string BgMainCity = "bgMainCity";
    public const string BgDungeon = "bgHuangYe";

    //登陆按钮名称
    public const string UiLoginBtn = "uiLoginBtn";
    public const string UiOpenPage = "uiOpenPage";

    //常规UI点击音效
    public const string UiClickBtn = "uiClickBtn";
    public const string MenuClickBtn = "uiExtenBtn";
    public const string StrengthenClickBtn = "fbitem";

    //屏幕标准宽高
    public const int ScreenStandardWidth = 1334;
    public const int ScreenStandardHeight = 750;

    //摇杆点标准距离
    public const int RockerOperateDistance = 90;

    //角色移动速度
    public const int PlayerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 4;

    //运动平滑
    public const float AcceleratedSpeed = 5f;

    //混合参数
    public const int BlendIdle = 0;
    public const int BlendMove = 1;

    //战斗动画的触发参数
    public const int ActionDefault = -1;


    //自动导航NPC
    public const int NpcGuide = -1;
    public const int NpcWiseMan = 0;
    public const int NpcGeneralMan = 1;
    public const int NpcArtisanMan = 2;
    public const int NpcTraderMan = 3;

    public static string Color(string str, TextColor color)
    {
        string result = "";
        switch (color)
        {
            case TextColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TextColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TextColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TextColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
        }

        return result;
    }
}