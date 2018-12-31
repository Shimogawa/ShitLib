using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using ShitLib.Net.Douyu.MessageTypes;

namespace ShitLib.Net.Douyu
{
	class DDanmakuGetter
	{
		private const string HOST = "openbarrage.douyutv.com";
		private const int PORT = 8601;
		private const int SEND_HEADER_CODE = 689;

		private int roomID;
		private bool isConnected;

		private TcpClient client;
		private NetworkStream stream;
		private Thread heartbeatThread;
		private Thread listenThread;

		public DanmakuList<DMessageType, DMessage> DanmakuList { get; }

		private DDanmakuGetter()
		{
			DanmakuList = new DanmakuList<DMessageType, DMessage>();
		}

		public DDanmakuGetter(int roomID) : this()
		{
			this.roomID = roomID;
		}

		public void Start()
		{
			Connect();
		}

		private void Connect()
		{
			client = new TcpClient();
			client.Connect(HOST, PORT);

			if (!client.Connected)
			{
				throw new Exception("Fuck me");
			}
			isConnected = true;
			DanmakuList.AddDanmaku(new MessageInfo<DMessageType, DMessage>(
				DMessageType.Log, new DMessage($"链接房间 {roomID} 成功")));
			stream = client.GetStream();

			heartbeatThread = new Thread(KeepHeartBeat);
			heartbeatThread.Start();
			listenThread = new Thread(KeepListen);
			listenThread.Start();
		}

		private void KeepListen()
		{
			var buffer = new byte[client.ReceiveBufferSize];
			while (isConnected)
			{
				try
				{
					stream.Read(buffer, 0, 4);

				}
				catch (Exception)
				{
					continue;
				}
			}
		}

		private void KeepHeartBeat()
		{
			while (isConnected)
			{
				long timeStamp = Utils.GetTimeStampSeconds();
				SendData($"type@=keeplive/tick@={timeStamp}/\0");
				Thread.Sleep(45000);
			}
		}

		private bool SendData(string msg)
		{
			var body = Encoding.UTF8.GetBytes(msg);
			var length = body.Length + 8;
			var sendData = new byte[length + 4];
			byte[] i32 = BitConverter.GetBytes(length);
			Array.Copy(i32, 0, sendData, 0, 4);
			Array.Copy(i32, 0, sendData, 4, 4);
			i32 = BitConverter.GetBytes(SEND_HEADER_CODE);
			Array.Copy(i32, 0, sendData, 8, 4);
			Array.Copy(body, 0, sendData, 12, body.Length);
			stream.Write(sendData, 0, sendData.Length);
			stream.Flush();
			return true;
		}

		private static readonly char[]     arraySplitter = new[] { '/' };
		private static readonly string[]   kvSplitter    = new[] { "@=" };
		private static Dictionary<string, string> ParseMessage(string msg)
		{
			var dict = new Dictionary<string, string>();
			var body = msg.Split(arraySplitter, StringSplitOptions.RemoveEmptyEntries);
			foreach (var s in body)
			{
				var ss = s.Split(kvSplitter, StringSplitOptions.RemoveEmptyEntries);

				ss[1] = ss[1].Replace("@A", "@").Replace("@S", "/");
				dict.Add(ss[0], ss[1]);
			}
			return dict;
		}
	}
}
