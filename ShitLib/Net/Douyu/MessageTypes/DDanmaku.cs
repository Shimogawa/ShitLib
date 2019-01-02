
using System.Text;

namespace ShitLib.Net.Douyu.MessageTypes
{
	public class DDanmaku : DMessage
	{
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
			if (user.Badge != null) sb.Append($"【{user.Badge.BadgeName}|{user.Badge.BadgeLevel}】");
			sb.Append($"{User.Username} 说： {danmaku}");
			
			WholeMessage = sb.ToString();
		}
	}
}
