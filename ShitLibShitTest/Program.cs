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
			linker = new DDanmakuGetter(78561);
			while (true)
			{
				linker.Connect();
				A();
			}
		}

		static void A()
		{
			while (linker.IsConnected)
			{
				if (linker.DanmakuList.IsEmpty()) continue;
				var f = linker.DanmakuList.GetFirst();
				//if (f.MessageType == DMessageType.Danmaku) continue;
				Console.WriteLine(f);
			}
		}
	}
}
