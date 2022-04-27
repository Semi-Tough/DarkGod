namespace Sever
{
    public class BaseData
    {
        public int Id;
    }

    public class GuideConfig : BaseData
    {
        public int Coin;
        public int Exp;
    }

    public class StrengthenConfig : BaseData
    {
        public int AddDefense;

        public int AddHp;
        public int AddHurt;
        public int Coin;
        public int Crystal;

        public int MinLevel;
        public int Position;
        public int StarLevel;
    }

    public class TaskConfig : BaseData
    {
        public int Coin;
        public int Exp;
        public int TargetCount;
    }

    public class MapConfig : BaseData
    {
        public int Power;
    }


    //用于数据库信息的解析
    public class TaskProgress : BaseData
    {
        public bool Finished;
        public int Progress;
    }
}