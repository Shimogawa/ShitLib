
namespace ShitLib.Net.Users
{
	public class Badge
	{
		public string BadgeName { get; internal set; }

		public int BadgeLevel { get; internal set; }

		public Badge() { }

		public Badge(string badgeName, int badgeLevel)
		{
			BadgeName = badgeName;
			BadgeLevel = badgeLevel;
		}
	}
}
