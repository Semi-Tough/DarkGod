using ProtoBuf;
using System.IO;

namespace PEProtocol {
	public class Protocol {
		public const int PowerAddCount = 5;
		public const int PowerAddInterval = 5 * 60 * 1000; //分钟


		public static byte[] Serialize<T>(T obj) where T : IExtensible, new() {
			using MemoryStream stream = new MemoryStream();
			Serializer.Serialize(stream, obj);
			return stream.ToArray();
		}

		public static T Deserialize<T>(byte[] bytes) where T : IExtensible, new() {
			using MemoryStream stream = new MemoryStream(bytes);
			return Serializer.Deserialize<T>(stream);
		}

		public static class SeverConfig {
			public const string SeverIp = "127.0.0.1";
			public const int SeverPort = 17666;
		}

		public static int GetFightByProperty(PlayerData playerData) {
			return playerData.Level * 100 +
			       playerData.Ad +
			       playerData.Ap +
			       playerData.AdDef +
			       playerData.ApDef;
		}

		public static int GetPowerLimit(int level) {
			return(level - 1) / 10 * 150 + 150;
		}
		public static int GetExpUpValueByLevel(int level) {
			return 100 * level * level;
		}

		public static void CalculateExp(PlayerData playerData, int addExp) {
			while(true) {
				int upgradeNeedExp = GetExpUpValueByLevel(playerData.Level) - playerData.Exp;
				if(addExp >= upgradeNeedExp) {
					playerData.Level += 1;
					playerData.Exp = 0;
					addExp -= upgradeNeedExp;
				}
				else {
					playerData.Exp += addExp;
					break;
				}
			}
		}
	}
}