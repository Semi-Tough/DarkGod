using Microsoft.EntityFrameworkCore;
using PEProtocol;
using PETool.PELogger;

namespace Server {
	public class DbSvc {
		public static DbSvc Instance { get; } = new DbSvc();



		public void Init() {
			using MyContext myContext = new MyContext();
			if(myContext.Database.CanConnect()) {
				PELogger.Log("Db Service Init Done.");
			}
			else {
				PELogger.Log("Database Connect Failed.");
			}
		}

		public bool AddNewPlayerData(PlayerData playerData) {
			try {
				using MyContext context = new MyContext();
				context.PlayerData.Add(playerData);
				context.SaveChanges();
			}
			catch(Exception e) {
				PELogger.Error(e);
				return false;
			}
			return true;
		}
		public bool UpdateDatabase(PlayerData player) {
			try {
				using MyContext context = new MyContext();
				context.PlayerData.Update(player);
				context.SaveChanges();
			}
			catch(Exception e) {
				PELogger.Error(e);
				return false;
			}
			return true;
		}
		public PlayerData? QueryPlayerData(string account) {
			using MyContext context = new MyContext();
			PlayerData? player = context.PlayerData.Include(c => c.StrengthenDatas).Include(c => c.TaskDatas).FirstOrDefault(e => e.Account == account);
			return player;
		}
		public bool QueryNameData(string name) {
			using MyContext context = new MyContext();
			PlayerData? player = context.PlayerData.FirstOrDefault(e => e.Name == name);
			return player != null;
		}
	}

	public class MyContext : DbContext {
		public DbSet<PlayerData> PlayerData { get; set; } = null!;
		public DbSet<StrengthenData> StrengthenData { get; set; } = null!;
		public DbSet<TaskData> TaskData { get; set; } = null!;

		override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlServer("Server=DESKTOP-15CQO55;Database=DarkGod;Integrated Security=True;TrustServerCertificate=true")
			              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
		}

		override protected void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<StrengthenData>(builder => {
				builder.Property(e => e.StrengthenType).HasConversion(
					s => s.ToString(),
					s => Enum.Parse<StrengthenType>(s)
				);
			});
		}
	}
}