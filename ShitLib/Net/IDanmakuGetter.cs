using System;

namespace ShitLib.Net
{
	public interface IDanmakuGetter
	{
		// TODO: Use global 'MessageType' enum.
		//DanmakuList<Enum, Message> DanmakuList { get; }

		bool Connect();

		void Disconnect();
	}
}
