
using System.Text;

namespace ShitLib.Net.Douyu.MessageTypes
{
	public class DDanmaku : DMessage
	{
		public static string[] COLOR_CODE =
		{
			"",
			"ff1e00",		// 1:RED
			"1e87f0",		// 2:BLUE
			"7ac84b",		// 3:GREEN
			"ff7f00",		// 4:ORANGE
			"9b38f4",		// 5:PURPLE
			"ff69b4",		// 6:PINK
		};

		public DUser User { get; }

		public string Danmaku { get; }

		public int Color { get; }

		public DDanmaku(DUser user, string danmaku, int color = 0)
		{
			User = user;
			Danmaku = danmaku;
			Color = color;

			var sb = new StringBuilder();
			if (user.IsSuperModerator) sb.Append("【超管】");
			if (user.IsRoomModerator) sb.Append("【房管】");
			if (user.Badge != null && user.Badge.BadgeLevel != 0) sb.Append($"【{user.Badge.BadgeName}|{user.Badge.BadgeLevel}】");
			if (user.NobelLevel.HasValue && user.NobelLevel.Value <= 6)
				sb.Append(
					$"【{(user.NobelLevel.Value <= 6 ? DUser.NOBEL_LEVEL_NAME[user.NobelLevel.Value] : user.NobelLevel.ToString())}】");
			sb.Append($"【Lv.{User.Level}】{User.Username} 说： {danmaku}");
			
			WholeMessage = sb.ToString();
		}
	}
}
