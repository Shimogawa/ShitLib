using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using ShitLib.Net.Bilibili.BLiveDanmaku;
using ShitLib.Net.Douyu;

namespace ShitLibShitTest
{
	class Program
	{
		private static DDanmakuGetter dlinker;
		private static BDanmakuGetter blinker;

		static void Main(string[] args)
		{
			BTest(388);
		}

		private static void BTest(int rm)
		{
			blinker = new BDanmakuGetter(rm);
			while (true)
			{
				blinker.Connect();
				BGetDanmaku();
			}
		}

		static void BGetDanmaku()
		{
			while (blinker.IsConnected)
			{
				if (blinker.DanmakuList.IsEmpty()) continue;
				var f = blinker.DanmakuList.GetFirst();
				//if (f.MessageType == DMessageType.Danmaku) continue;
				Console.WriteLine(f);
			}
		}

		static void TestBHTTPGetter(int room)
		{
			var request = (HttpWebRequest)WebRequest.Create("https://live.bilibili.com/" + room.ToString());
			request.Method = "GET";
			request.Connection = "keep_alive";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
			request.UserAgent =
				"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
			request.Timeout = 10000;
			string response;
			var a = request.GetResponse();
			using (var responseStream = a.GetResponseStream())
			{
				using (var reader = new StreamReader(responseStream, Encoding.UTF8))
				{
					response = reader.ReadToEnd();
				}
			}

			var regex = new Regex("\"room_id\":(\\d+)");
			var found = regex.Match(response).Value;
			var trueID = int.Parse(found.Substring(found.IndexOf(':') + 1));
			Console.WriteLine(trueID);

			request = (HttpWebRequest)WebRequest.Create("http://live.bilibili.com/api/player?id=cid:" + trueID);
			request.Method = "GET";
			request.Connection = "keep_alive";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
			request.UserAgent =
				"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
			request.Timeout = 10000;
			using (var responseStream = request.GetResponse().GetResponseStream())
			{
				using (var reader = new StreamReader(responseStream, Encoding.UTF8))
				{
					response = reader.ReadToEnd();
				}
			}

			var doc = new XmlDocument();
			response = "<root>" + response + "</root>";
			doc.LoadXml(response);
			var host = doc["root"]["dm_server"].InnerText;
			//            Console.WriteLine(_host);
			var state = doc["root"]["state"].InnerText;
			//            Console.WriteLine(state);
			var port = int.Parse(doc["root"]["dm_port"].InnerText);
			Console.WriteLine($"{host}:{port}, {state}");
		}
	}
}
