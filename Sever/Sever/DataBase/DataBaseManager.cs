/****************************************************
	文件：DataBaseManager.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/03/01 9:57   	
	功能：数据库管理类
*****************************************************/

using System;
using MySql.Data.MySqlClient;
using PEProtocol;

namespace Sever
{
    internal class DataBaseManager
    {
        private static DataBaseManager _instance;

        private MySqlConnection _conn;
        public static DataBaseManager Instance => _instance ?? (_instance = new DataBaseManager());

        public void Init()
        {
            _conn = new MySqlConnection(
                "server=localhost; userid=root; password=; database= darkgod; charset=utf8");
            _conn.Open();
            PeCommon.Log("DataBaseManager Init Done.");
        }

        // 根据账号密码查询对应的账号数据
        public PlayerData QueryPlayerData(string account, string password)
        {
            PlayerData playerData = null;
            MySqlDataReader reader = null;
            bool createNewAccount = true;
            try
            {
                MySqlCommand cmd =
                    new MySqlCommand("select*from account where account=@account", _conn);
                cmd.Parameters.AddWithValue("account", account);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    createNewAccount = false;
                    string pass = reader.GetString("password");
                    //密码正确,返回玩家数据
                    if (pass.Equals(password))
                    {
                        playerData = new PlayerData
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Level = reader.GetInt32("level"),
                            Exp = reader.GetInt32("exp"),
                            Power = reader.GetInt32("power"),
                            Coin = reader.GetInt32("coin"),
                            Diamond = reader.GetInt32("diamond"),
                            Crystal = reader.GetInt32("crystal"),

                            Hp = reader.GetInt32("hp"),
                            Ad = reader.GetInt32("ad"),
                            Ap = reader.GetInt32("ap"),
                            AdDef = reader.GetInt32("addef"),
                            ApDef = reader.GetInt32("apdef"),
                            Dodge = reader.GetInt32("dodge"),
                            Pierce = reader.GetInt32("pierce"),
                            Critical = reader.GetInt32("critical"),

                            GuideId = reader.GetInt32("guideid"),
                            Time = reader.GetInt64("time"),
                            DungeonId = reader.GetInt32("dungeon")
                        };

                        //字符串存储强化等级
                        //头部级别 # 身体级别 # 腰部级别 # 手部级别 # 腿部级别 # 脚本级别 # 
                        string[] strengthenStrArr = reader.GetString("strengthen").Split('#');
                        int[] strengthen = new int[6];
                        for (int i = 0; i < strengthenStrArr.Length; i++)
                        {
                            //#分割后,最后一位为空白
                            if (string.IsNullOrWhiteSpace(strengthenStrArr[i])) continue;

                            if (strengthenStrArr.Length < 6) throw new Exception("Read Strengthen Array Error.");

                            if (int.TryParse(strengthenStrArr[i], out int starLevel))
                                strengthen[i] = starLevel;
                            else
                                PeCommon.Log("Parse strengthen data error", PeLogType.Error);
                        }

                        playerData.StrengthenArr = strengthen;

                        //字符串存储任务奖励信息
                        /* 任务ID|任务进度|任务是否已完成 # 任务ID|任务进度|任务是否已完成 # ...*/
                        string[] taskArr = reader.GetString("task").Split('#');
                        playerData.TaskArr = new string[6];

                        for (int i = 0; i < taskArr.Length; i++)
                        {
                            //#分割后,最后一位为空白
                            if (string.IsNullOrWhiteSpace(taskArr[i])) continue;
                            if (taskArr.Length < 6) throw new Exception("Read TaskArray Error.");

                            playerData.TaskArr[i] = taskArr[i];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PeCommon.Log("Query PlayerData By Account && Password Error: " + e, PeLogType.Error);
            }
            finally
            {
                reader?.Close();
                if (createNewAccount)
                {
                    // 账户不存在时默认创建该账户
                    playerData = new PlayerData
                    {
                        Id = -1,
                        Name = "",
                        Level = 1,

                        Exp = 0,
                        Coin = 5000,
                        Diamond = 500,
                        Crystal = 500,

                        Hp = 2000,
                        Ad = 275,
                        Ap = 265,
                        AdDef = 67,
                        ApDef = 43,
                        Dodge = 7,
                        Pierce = 5,
                        Critical = 2,

                        Power = 150,
                        Time = TimerService.Instance.GetNowTime(),

                        GuideId = 1001,
                        StrengthenArr = new int[6],
                        TaskArr = new string[6],
                        DungeonId = 10001
                    };
                    for (int i = 0; i < playerData.TaskArr.Length; i++)
                        playerData.TaskArr[i] = i + 1 + "|0|0";

                    playerData.Id = InsertNewAccountData(account, password, playerData);
                }
            }

            //密码错误,返回null (playerData未赋值)
            return playerData;
        }

        private int InsertNewAccountData(string account, string password, PlayerData playerData)
        {
            int id = -1;
            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(
                    "insert into account set account=@account,password=@password,name=@name,level=@level," +
                    "exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal,hp=@hp,ad=@ad,ap=@ap,addef=@addef," +
                    "apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strengthen=@strengthen," +
                    "time=@time,task=@task,dungeon=@dungeon", _conn);
                sqlCommand.Parameters.AddWithValue("account", account);
                sqlCommand.Parameters.AddWithValue("password", password);
                sqlCommand.Parameters.AddWithValue("name", playerData.Name);
                sqlCommand.Parameters.AddWithValue("level", playerData.Level.ToString());
                sqlCommand.Parameters.AddWithValue("exp", playerData.Exp.ToString());
                sqlCommand.Parameters.AddWithValue("power", playerData.Power.ToString());
                sqlCommand.Parameters.AddWithValue("coin", playerData.Coin.ToString());
                sqlCommand.Parameters.AddWithValue("diamond", playerData.Diamond.ToString());
                sqlCommand.Parameters.AddWithValue("crystal", playerData.Crystal.ToString());

                sqlCommand.Parameters.AddWithValue("hp", playerData.Hp.ToString());
                sqlCommand.Parameters.AddWithValue("ad", playerData.Ad.ToString());
                sqlCommand.Parameters.AddWithValue("ap", playerData.Ap.ToString());
                sqlCommand.Parameters.AddWithValue("addef", playerData.AdDef.ToString());
                sqlCommand.Parameters.AddWithValue("apdef", playerData.ApDef.ToString());
                sqlCommand.Parameters.AddWithValue("dodge", playerData.Dodge.ToString());
                sqlCommand.Parameters.AddWithValue("pierce", playerData.Pierce.ToString());
                sqlCommand.Parameters.AddWithValue("critical", playerData.Critical.ToString());

                sqlCommand.Parameters.AddWithValue("guideid", playerData.GuideId.ToString());

                #region AddStrengthenArr

                string strengthenInfo = "";
                foreach (int level in playerData.StrengthenArr)
                {
                    strengthenInfo += level.ToString();
                    strengthenInfo += "#";
                }

                sqlCommand.Parameters.AddWithValue("strengthen", strengthenInfo);

                #endregion

                sqlCommand.Parameters.AddWithValue("time", playerData.Time.ToString());

                #region AddTaskArr

                string taskInfo = "";
                foreach (string task in playerData.TaskArr)
                {
                    taskInfo += task;
                    taskInfo += "#";
                }

                sqlCommand.Parameters.AddWithValue("task", taskInfo);

                #endregion

                sqlCommand.Parameters.AddWithValue("dungeon", playerData.DungeonId.ToString());


                sqlCommand.ExecuteNonQuery();
                id = (int) sqlCommand.LastInsertedId;
            }
            catch (Exception e)
            {
                PeCommon.Log("Insert PlayerData Error: " + e, PeLogType.Error);
            }

            return id;
        }

        public bool QueryNameData(string name)
        {
            bool exist = false;
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select*from account where name=@name", _conn);
                cmd.Parameters.AddWithValue("name", name);
                cmd.ExecuteNonQuery();
                reader = cmd.ExecuteReader();
                if (reader.Read()) exist = true;
            }
            catch (Exception e)
            {
                PeCommon.Log("Query Name State Error: " + e);
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            return exist;
        }

        public bool UpDataPlayerData(int id, PlayerData playerData)
        {
            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(
                    "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin," +
                    "diamond=@diamond,crystal=@crystal,hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge," +
                    "pierce=@pierce,critical=@critical,guideid=@guideid,strengthen=@strengthen,time=@time,task=@task," +
                    "dungeon=@dungeon" + " where id=@id",
                    _conn);
                sqlCommand.Parameters.AddWithValue("id", id.ToString());
                sqlCommand.Parameters.AddWithValue("name", playerData.Name);
                sqlCommand.Parameters.AddWithValue("level", playerData.Level.ToString());
                sqlCommand.Parameters.AddWithValue("exp", playerData.Exp.ToString());
                sqlCommand.Parameters.AddWithValue("power", playerData.Power.ToString());
                sqlCommand.Parameters.AddWithValue("coin", playerData.Coin.ToString());
                sqlCommand.Parameters.AddWithValue("diamond", playerData.Diamond.ToString());
                sqlCommand.Parameters.AddWithValue("crystal", playerData.Crystal.ToString());

                sqlCommand.Parameters.AddWithValue("hp", playerData.Hp.ToString());
                sqlCommand.Parameters.AddWithValue("ad", playerData.Ad.ToString());
                sqlCommand.Parameters.AddWithValue("ap", playerData.Ap.ToString());
                sqlCommand.Parameters.AddWithValue("addef", playerData.AdDef.ToString());
                sqlCommand.Parameters.AddWithValue("apdef", playerData.ApDef.ToString());
                sqlCommand.Parameters.AddWithValue("dodge", playerData.Dodge.ToString());
                sqlCommand.Parameters.AddWithValue("pierce", playerData.Pierce.ToString());
                sqlCommand.Parameters.AddWithValue("critical", playerData.Critical.ToString());

                sqlCommand.Parameters.AddWithValue("guideid", playerData.GuideId.ToString());

                #region AddStrengthenArr

                string strengthenInfo = "";
                foreach (int level in playerData.StrengthenArr)
                {
                    strengthenInfo += level.ToString();
                    strengthenInfo += "#";
                }

                sqlCommand.Parameters.AddWithValue("strengthen", strengthenInfo);

                #endregion

                sqlCommand.Parameters.AddWithValue("time", playerData.Time.ToString());

                #region AddTaskArr

                string taskInfo = "";
                foreach (string task in playerData.TaskArr)
                {
                    taskInfo += task;
                    taskInfo += "#";
                }

                sqlCommand.Parameters.AddWithValue("task", taskInfo);

                #endregion

                sqlCommand.Parameters.AddWithValue("dungeon", playerData.DungeonId.ToString());


                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                PeCommon.Log("UpDate PlayerData Error: " + e);
                return false;
            }

            return true;
        }
    }
}