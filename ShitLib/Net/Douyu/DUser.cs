
using System.Collections.Generic;
using System.Text;

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
			"公爵",
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

		public override string ToString()
		{
			return GetString(this, true, true, true);
		}

		public static string GetString(DUser user, bool showModeratorStatus, bool showNobelLevel, bool showBadge)
		{
			var sb = new StringBuilder();
			if (showModeratorStatus && user.IsSuperModerator) sb.Append("【超管】");
			if (showModeratorStatus && user.IsRoomModerator) sb.Append("【房管】");
			if (showNobelLevel && user.NobelLevel.HasValue && user.NobelLevel.Value <= 6)
				sb.Append(
					$"【{(user.NobelLevel.Value <= 6 ? NOBEL_LEVEL_NAME[user.NobelLevel.Value] : user.NobelLevel.ToString())}】");
			if (showBadge && user.Badge != null && user.Badge.BadgeLevel != 0)
				sb.Append($"【{user.Badge.BadgeName}|{user.Badge.BadgeLevel}】");
			sb.Append($"【Lv.{user.Level}】{user.Username}");
			return sb.ToString();
		}
	}
}
