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
	public class DDanmakuGetter : IDanmakuGetter
	{
		private const string HOST = "openbarrage.douyutv.com";
		private const string ICON_URL = "https://apic.douyucdn.cn/upload/{0}_big.jpg";
		private const int PORT = 8601;
		private const int SEND_HEADER_CODE = 689;
		private const int SERVER_HEADER_CODE = 690;

		private int roomID;
		private bool isConnected;

		private TcpClient client;
		private NetworkStream stream;
		private Thread heartbeatThread;
		private Thread listenThread;

		public DanmakuList<MessageType, DMessage> DanmakuList { get; }

		private DDanmakuGetter()
		{
			DanmakuList = new DanmakuList<MessageType, DMessage>();
		}

		public DDanmakuGetter(int roomID) : this()
		{
			this.roomID = roomID;
		}

		public bool Connect()
		{
			return internal_Connect();
		}

		public void Disconnect()
		{
			if (isConnected)
			{
				Logout();
				isConnected = false;
				client.Close();
				stream.Close();
			}
		}

		private bool internal_Connect()
		{
			client = new TcpClient();
			client.Connect(HOST, PORT);

			if (!client.Connected)
			{
				return false;
				//throw new Exception("Fuck me");
			}
			isConnected = true;
			stream = client.GetStream();
			listenThread = new Thread(KeepListen);
			listenThread.Start();

			EnterRoom();
			EnterGroup();

			heartbeatThread = new Thread(KeepHeartBeat);
			heartbeatThread.Start();
			return true;
		}

		private void EnterRoom()
		{
			SendData($"type@=loginreq/roomid@={roomID}/\0");
		}

		private void EnterGroup()
		{
			SendData($"type@=joingroup/rid@={roomID}/gid@=-9999/\0");
		}

		private void Logout()
		{
			SendData("type@=logout/\0");
		}

		private void KeepListen()
		{
			var buffer = new byte[client.ReceiveBufferSize];
			while (isConnected)
			{
				try
				{
					stream.Read(buffer, 0, 4);
					var len1 = BitConverter.ToInt32(buffer, 0);
					stream.Read(buffer, 4, 4);
					var len2 = BitConverter.ToInt32(buffer, 4);
					stream.Read(buffer, 8, 4);
					var msgType = BitConverter.ToInt32(buffer, 8);
					//if (msgType != SERVER_HEADER_CODE)
					//	throw new Exception("Wrong message.");
					//stream.Read(buffer, 12, len1 - 8);
					stream.ReadB(buffer, 12, len1 - 8);
					var body = Encoding.UTF8.GetString(buffer, 12, len1 - 8);
					var dict = ParseMessage(body);
					switch (dict["type"])
					{
						case "loginres":    // Login result
							DanmakuList.AddDanmaku(new MessageInfo<MessageType, DMessage>(
								MessageType.EnterRoom, new DMessage($"链接房间 {roomID} 成功")));
							break;
						case "chatmsg":     // Danmaku
							int color = dict.ContainsKey("col") ? int.Parse(dict["col"]) : 0;
							var user = GetUser(dict);
							DanmakuList.AddDanmaku(new MessageInfo<MessageType, DMessage>(
								MessageType.Danmaku, new DDanmaku(user, dict["txt"], color)));
							break;
						case "dgb":			// Gifts
							var amount = dict.ContainsKey("gfcnt") ? int.Parse(dict["gfcnt"]) : 1;
							user = GetUser(dict);
							var hits = dict.ContainsKey("hits") ? int.Parse(dict["hits"]) : 1;
							DanmakuList.AddDanmaku(new MessageInfo<MessageType, DMessage>(
								MessageType.Gift, new DGift(user, int.Parse(dict["gfid"]), amount, hits)));
							break;
						default:
							break;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
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
			var length = body.Length + 9;
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

		private static DUser GetUser(IReadOnlyDictionary<string, string> dict)
		{
			var icon = dict.ContainsKey("ic") ? string.Format(ICON_URL, dict["ic"]) : null;
			bool rg = dict.ContainsKey("rg") && int.Parse(dict["rg"]) > 3;
			bool pg = dict.ContainsKey("pg");
			var badge = dict.ContainsKey("bnn") ? new DBadge(dict["bnn"], int.Parse(dict["bl"])) : null;
			int? nobelLevel = dict.ContainsKey("nl") ? (int?)int.Parse(dict["nl"]) : null;

			return new DUser(
				dict["nn"], int.Parse(dict["uid"]), int.Parse(dict["level"]),
				icon, rg, pg, nobelLevel, badge);
		}

		private static readonly char[]     arraySplitter = new[] { '/' };
		private static readonly string[]   kvSplitter    = new[] { "@=" };
		private static Dictionary<string, string> ParseMessage(string msg)
		{
			var dict = new Dictionary<string, string>();
			var body = msg.Split(arraySplitter, StringSplitOptions.RemoveEmptyEntries);
			foreach (var s in body)
			{
				if (s == "\0") break;
				var ss = s.Split(kvSplitter, StringSplitOptions.None);
				if (ss.Length != 2) continue;
				ss[1] = ss[1].Replace("@A", "@").Replace("@S", "/");
				dict.Add(ss[0], ss[1]);
			}
			return dict;
		}
	}
}
