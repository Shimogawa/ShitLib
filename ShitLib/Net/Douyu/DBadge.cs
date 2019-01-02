
namespace ShitLib.Net.Douyu
{
	public class DBadge
	{
		public string BadgeName { get; }

		public int BadgeLevel { get; }

		public DBadge(string badgeName, int badgeLevel)
		{
			BadgeName = badgeName;
			BadgeLevel = badgeLevel;
		}
	}
}
