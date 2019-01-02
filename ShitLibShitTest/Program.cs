using System;
using System.Threading;
using ShitLib.Net.Bilibili.BLiveDanmaku;
using ShitLib.Net.Douyu;

namespace ShitLibShitTest
{
	class Program
	{
		private static DDanmakuGetter linker;

		static void Main(string[] args)
		{
			linker = new DDanmakuGetter(-5);
			new Thread(() => linker.Connect()).Start();
			new Thread(A).Start();
		}

		static void A()
		{
			while (true)
			{
				if (linker.DanmakuList.IsEmpty()) continue;
				var f = linker.DanmakuList.GetFirst();
				//if (f.MessageType == DMessageType.Danmaku) continue;
				Console.WriteLine(f);
			}
		}
	}
}
