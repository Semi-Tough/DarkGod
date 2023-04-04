/****************************************************
    文件：CacheService
    作者：Semi-Tough
    邮箱：1693416984@qq.com
    日期：2022年02月28日 星期一 12:20 
    功能：缓存服务
*****************************************************/

using PEProtocol;
using PETool.PELogger;
using Server;

namespace Sever {
	public class CacheSvc {
		public static CacheSvc Instance { get; } = new CacheSvc();

		private readonly Dictionary<string, SeverSession> onLineAccountDic =
			new Dictionary<string, SeverSession>();

		private readonly Dictionary<SeverSession, PlayerData> onLineSessionDic =
			new Dictionary<SeverSession, PlayerData>();


		public void Init() {
			// dataBaseManager = DataBaseManager.Instance;
			PELogger.Log("Catch Service Init Done.");
		}

		public bool IsAccountOnLine(string account) {
			return onLineAccountDic.ContainsKey(account);
		}

		public bool IsNameExist(string name) {
			// 在数据库中查询名字是否已存在
			return DbSvc.Instance.QueryNameData(name);
			// return dataBaseManager.QueryNameData(name);
		}

		public void AccountOnline(string account, SeverSession session, PlayerData playerData) {
			// 缓存上线账号的数据
			onLineAccountDic.Add(account, session);
			onLineSessionDic.Add(session, playerData);
		}

		public void AccountOffline(SeverSession session) {
			foreach(KeyValuePair<string, SeverSession> account in onLineAccountDic)
				if(account.Value == session) {
					onLineAccountDic.Remove(account.Key);
					break;
				}


			onLineSessionDic.Remove(session);
			PELogger.Log("Offline SessionID: " + session.sessionId);
		}

		public List<SeverSession> GetOnLineSeverSession() {
			List<SeverSession> sessionList = new List<SeverSession>();

			foreach(KeyValuePair<SeverSession, PlayerData> session in onLineSessionDic)
				sessionList.Add(session.Key);

			return sessionList;
		}

		public PlayerData? GetPlayerDataByAccount(string account) {
			//从数据库中查找账户
			// return dataBaseManager.QueryPlayerData(account, password);
			return DbSvc.Instance.QueryPlayerData(account);
		}

		public PlayerData? GetPlayerDataBySession(SeverSession session) {
			return onLineSessionDic.TryGetValue(session, out PlayerData? playerData)
				? playerData
				: null;
		}

		public SeverSession? GetSessionByPlayerId(int playerId) {
			SeverSession? session = null;
			foreach(KeyValuePair<SeverSession, PlayerData> playerData in onLineSessionDic) {
				if(playerData.Value != null && playerData.Value.Id != playerId) continue;
				session = playerData.Key;
				break;
			}

			return session;
		}

		public Dictionary<SeverSession, PlayerData> GetOnlineCache() {
			return onLineSessionDic;
		}

		public bool UpdateDatabase(PlayerData playerData) {
			//数据库更新
			return DbSvc.Instance.UpdateDatabase(playerData);
			// return dataBaseManager.UpDataPlayerData(playerId, playerData);
		}
	}
}