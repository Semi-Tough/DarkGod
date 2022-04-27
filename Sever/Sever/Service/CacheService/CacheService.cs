/****************************************************
    文件：CacheService
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年02月28日 星期一 12:20 
    功能：缓存服务
*****************************************************/

using System.Collections.Generic;
using PEProtocol;

namespace Sever
{
    public class CacheService
    {
        private static CacheService _instance;

        private readonly Dictionary<string, SeverSession> _onLineAccountDic =
            new Dictionary<string, SeverSession>();

        private readonly Dictionary<SeverSession, PlayerData> _onLineSessionDic =
            new Dictionary<SeverSession, PlayerData>();

        private DataBaseManager _dataBaseManager;

        public static CacheService Instance => _instance ?? (_instance = new CacheService());

        public void Init()
        {
            _dataBaseManager = DataBaseManager.Instance;
            PeCommon.Log("Catch Service Done.");
        }

        public bool IsAccountOnLine(string account)
        {
            return _onLineAccountDic.ContainsKey(account);
        }

        public bool IsNameExist(string name)
        {
            // 在数据库中查询名字是否已存在
            return _dataBaseManager.QueryNameData(name);
        }

        public void AccountOnline(string account, SeverSession session, PlayerData playerData)
        {
            // 缓存上线账号的数据
            _onLineAccountDic.Add(account, session);
            _onLineSessionDic.Add(session, playerData);
        }

        public void AccountOffline(SeverSession session)
        {
            foreach (KeyValuePair<string, SeverSession> account in _onLineAccountDic)
                if (account.Value == session)
                {
                    _onLineAccountDic.Remove(account.Key);
                    break;
                }


            _onLineSessionDic.Remove(session);
            PeCommon.Log("Offline SessionID: " + session.SessionId);
        }

        public List<SeverSession> GetOnLineSeverSession()
        {
            List<SeverSession> sessionList = new List<SeverSession>();

            foreach (KeyValuePair<SeverSession, PlayerData> session in _onLineSessionDic)
                sessionList.Add(session.Key);

            return sessionList;
        }

        public PlayerData GetPlayerDataByAccount(string account, string password)
        {
            //从数据库中查找账户
            return _dataBaseManager.QueryPlayerData(account, password);
        }

        public PlayerData GetPlayerDataBySession(SeverSession session)
        {
            return _onLineSessionDic.TryGetValue(session, out PlayerData playerData)
                ? playerData
                : null;
        }

        public SeverSession GetSessionByPlayerId(int playerId)
        {
            SeverSession session = null;
            foreach (KeyValuePair<SeverSession,PlayerData> playerData in _onLineSessionDic)
            {
                if (playerData.Value.Id != playerId) continue;
                session = playerData.Key;
                break;
            }

            return session;
        }

        public Dictionary<SeverSession, PlayerData> GetOnlineCache()
        {
            return _onLineSessionDic;
        }

        public bool UpDataPlayerDataToDataBase(int playerId, PlayerData playerData)
        {
            //数据库更新
            return _dataBaseManager.UpDataPlayerData(playerId, playerData);
        }
    }
}