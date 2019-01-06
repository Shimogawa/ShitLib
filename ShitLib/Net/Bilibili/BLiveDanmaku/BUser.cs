
using ShitLib.Net.Users;

namespace ShitLib.Net.Bilibili.BLiveDanmaku
{
	public class BUser
	{
		public const string SVIP = "爷";

		public string Username { get; }

		public bool IsAdmin { get; }

		public bool IsSVIP { get; }

		public Badge Badge { get; }

		public BUser(string username, bool isAdmin, bool isSvip, Badge badge)
		{
			Username = username;
			IsAdmin = isAdmin;
			IsSVIP = isSvip;
			Badge = badge;
		}
	}
}
