using ShitLib.Net.MessageTypes;

namespace ShitLib.Net.Douyu.MessageTypes
{
	public class DMessage : Message
	{
		public DMessage() { }

		public DMessage(string msg, string username = null)
		{
			WholeMessage = msg;
			Username = username;
		}
	}
}
