using System.Text;

namespace ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes
{
	public class BDanmaku : BMessage
	{
		public BUser User { get; }

		public string Danmaku { get; }

		public BDanmaku(BUser user, string danmaku)
		{
			User = user;
			Username = user.Username;
			Danmaku = danmaku;

			var sb = new StringBuilder();
			if (user.IsAdmin) sb.Append("【房管】");
			if (user.IsSVIP) sb.Append("【爷】");
			if (user.Badge != null)
				sb.Append(
					string.Format("【{0} {1}】", user.Badge.BadgeName, user.Badge.BadgeLevel
				));

			sb.Append($"{user.Username} 说： {danmaku}");
			WholeMessage = sb.ToString();
		}
	}
}
