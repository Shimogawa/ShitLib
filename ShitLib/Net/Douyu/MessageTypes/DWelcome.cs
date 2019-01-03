
namespace ShitLib.Net.Douyu.MessageTypes
{
	public class DWelcome : DMessage
	{
		public DUser User { get; }

		public DWelcome(DUser user)
		{
			User = user;
			WholeMessage = $"{DUser.GetString(user, true, true, false)} 进入直播间。";
		}
	}
}
