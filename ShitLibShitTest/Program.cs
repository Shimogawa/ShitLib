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
			linker = new DDanmakuGetter(5630372);
			new Thread(() => linker.Connect()).Start();
			new Thread(A).Start();
		}

		static void A()
		{
			foreach (var info in linker.DanmakuList.KeepGetting())
			{
				Console.WriteLine(info.Message.WholeMessage);
			}
		}
	}
}
