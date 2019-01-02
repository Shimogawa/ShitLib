
namespace ShitLib.Net
{
	public class MessageInfo<TMsgType, TMsg>
	{
		public TMsgType MessageType { get; }

		public TMsg Message { get; }

		private MessageInfo() { }

		public MessageInfo(TMsgType type, TMsg message)
		{
			MessageType = type;
			Message = message;
		}

		public override string ToString()
		{
			return Message.ToString();
		}
	}
}
