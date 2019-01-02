
namespace ShitLib.Net
{
	public class Message
	{
		public string Username { get; internal set; }

		public string WholeMessage { get; internal set; }

		public Message() { }

		public Message(string msg, string username = null)
		{
			WholeMessage = msg;
			Username = username;
		}

		public override string ToString()
		{
			return WholeMessage;
		}
	}
}
