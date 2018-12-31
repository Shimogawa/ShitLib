using System.Collections.Generic;

namespace ShitLib.Net
{
	public class DanmakuList<TMsgType, TMsg>
	{
		private LinkedList<MessageInfo<TMsgType, TMsg>> danmakuList = new LinkedList<MessageInfo<TMsgType, TMsg>>();

		public DanmakuList()
		{
		}

		public void AddDanmaku(MessageInfo<TMsgType, TMsg> danmaku)
		{
			danmakuList.AddLast(danmaku);
		}

		public IEnumerable<MessageInfo<TMsgType, TMsg>> KeepGetting()
		{
			MessageInfo<TMsgType, TMsg> temp;
			while (true)
			{
				if (!IsEmpty())
				{
					temp = danmakuList.First.Value;
					danmakuList.RemoveFirst();
					yield return temp;
				}
			}
		}

		public MessageInfo<TMsgType, TMsg> GetFirst()
		{
			if (IsEmpty()) return null;
			var temp = danmakuList.First.Value;
			danmakuList.RemoveFirst();
			return temp;
		}

		public bool IsEmpty()
		{
			return danmakuList.Count == 0;
		}
	}
}
