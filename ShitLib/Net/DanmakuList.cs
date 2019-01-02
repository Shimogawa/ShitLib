using System.Collections.Generic;

namespace ShitLib.Net
{
	public class DanmakuList<TMsgType, TMsg>
	{
		private Queue<MessageInfo<TMsgType, TMsg>> danmakuList = new Queue<MessageInfo<TMsgType, TMsg>>();

		public DanmakuList()
		{
		}

		public void AddDanmaku(MessageInfo<TMsgType, TMsg> danmaku)
		{
			danmakuList.Enqueue(danmaku);
		}

		public IEnumerable<MessageInfo<TMsgType, TMsg>> KeepGetting()
		{
			MessageInfo<TMsgType, TMsg> temp;
			while (true)
			{
				if (!IsEmpty())
				{
					temp = danmakuList.Dequeue();
					yield return temp;
				}
			}
		}

		public MessageInfo<TMsgType, TMsg> GetFirst()
		{
			if (IsEmpty()) return null;
			var temp = danmakuList.Dequeue();
			return temp;
		}

		public bool IsEmpty()
		{
			return danmakuList.Count == 0;
		}
	}
}
