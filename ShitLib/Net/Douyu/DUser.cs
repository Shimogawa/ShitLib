
using System.Collections.Generic;

namespace ShitLib.Net.Douyu
{
	public class DUser
	{
		public static readonly string[] NOBEL_LEVEL_NAME =
		{
			"游侠",
			"骑士",
			"子爵",
			"伯爵",
			"勋爵",
			"国王",
			"皇帝"
		};

		public string Username { get; }

		public int Uid { get; }

		public int Level { get; }

		public string Icon { get; }

		public bool IsRoomModerator { get; }

		public bool IsSuperModerator { get; }

		public int? NobelLevel { get; }

		public DBadge Badge { get; }

		public DUser(string username, int uid, int level, string icon, bool rg, bool pg, int? nl, DBadge badge)
		{
			Username = username;
			Uid = uid;
			Level = level;
			Icon = icon;
			IsRoomModerator = rg;
			IsSuperModerator = pg;
			NobelLevel = nl;
			Badge = badge;
		}
	}
}
