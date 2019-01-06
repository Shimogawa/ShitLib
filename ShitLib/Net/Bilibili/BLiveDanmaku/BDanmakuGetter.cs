using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Newtonsoft.Json.Linq;
using ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes;

namespace ShitLib.Net.Bilibili.BLiveDanmaku
{
	public class BDanmakuGetter : IDanmakuGetter
	{
		private const string LIVE_URL =           "http://live.bilibili.com/";
		private const string CID_URL =            "http://live.bilibili.com/api/player?id=cid:";
		private const string GET =                "GET";
		private const string REQUEST_CONNECTION = "keep_alive";
		private const string REQUEST_USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
		private const string REQUEST_ACCEPT =     "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
		private const string DEFAULT_HOST =       "livecmt-1.bilibili.com";
		private const int DEFAULT_PORT =          2243;
		private const int PROTOCOL_VERSION =      1;

		private static readonly bool isLittleEndian = BitConverter.IsLittleEndian;

		private string _host = DEFAULT_HOST;
		private int _port = DEFAULT_PORT;

		private int _roomUrlID;
		private int _trueID;
		private long _fake_uid;

		public bool IsConnected { get; private set; }

		public LiveState State { get; private set; }

		private TcpClient _client;
		private NetworkStream _stream;

		public DanmakuList<MessageType, BMessage> DanmakuList { get; private set; }

		public BDanmakuGetter(int roomUrlID)
		{
			_roomUrlID = roomUrlID;
			DanmakuList = new DanmakuList<MessageType, BMessage>();
		}

		public bool Connect()
		{
			while (true)
			{
				try
				{
					internal_Connect();
					break;
				}
				catch (TimeoutException)
				{
					DanmakuList.AddDanmaku(new MessageInfo<MessageType, BMessage>(
						MessageType.Log, new BOtherMsg("Request time out. Trying again.")));
				}
				catch (WebException)
				{
					DanmakuList.AddDanmaku(new MessageInfo<MessageType, BMessage>(
						MessageType.Log, new BOtherMsg("Web error. Connection failed.")));
					return false;
				}
			}

			if (State == LiveState.Offline)
			{
				DanmakuList.AddDanmaku(new MessageInfo<MessageType, BMessage>(
					MessageType.Log, new BOtherMsg("主播不在线。")));
				return false;
			}
			StartListen();
			return true;
		}

		private void internal_Connect()
		{
			try
			{
				var request = (HttpWebRequest)WebRequest.Create(LIVE_URL + _roomUrlID.ToString());
				request.Method = GET;
				request.Connection = REQUEST_CONNECTION;
				request.Accept = REQUEST_ACCEPT;
				request.UserAgent = REQUEST_USER_AGENT;
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
				var found = regex.Matches(response)[0].Value;
				_trueID = int.Parse(found.Substring(found.IndexOf(':') + 1));

				request = (HttpWebRequest)WebRequest.Create(CID_URL + _trueID);
				request.Method = GET;
				request.Connection = REQUEST_CONNECTION;
				request.Accept = REQUEST_ACCEPT;
				request.UserAgent = REQUEST_USER_AGENT;
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
				_host = doc["root"]["dm_server"].InnerText;
				_port = int.Parse(doc["root"]["dm_port"].InnerText);
				var state = doc["root"]["state"].InnerText;

				if (state == "LIVE")
					State = LiveState.Live;
				else if (state == "ROUND")
					State = LiveState.Round;
				else
				{
					State = LiveState.Offline;
					return;
				}

				IsConnected = true;
			}
			catch (Exception)
			{
				_trueID = _roomUrlID;
				_host = DEFAULT_HOST;
				_port = DEFAULT_PORT;
				State = LiveState.Live;
				IsConnected = true;
			}
		}

		private void StartListen()
		{
			_client = new TcpClient();
			//            var endpoint = new IPEndPoint(IPAddress.Parse(_host), PORT);
			//            _socket.Connect(_host, _port);

			_client.Connect(_host, _port);

			if (!_client.Connected)
			{
				throw new Exception("Fuck me");
			}

			_stream = _client.GetStream();

			var listenThread = new Thread(KeepListen);
			//            listenThread.IsBackground = true;
			listenThread.Start();

			// get into the room
			JoinChannel();
			var heartBeatThread = new Thread(KeepHeartBeat);
			//            heartBeatThread.IsBackground = true;
			heartBeatThread.Start();

			DanmakuList.AddDanmaku(new MessageInfo<MessageType, BMessage>(
				MessageType.Log, new BOtherMsg($"链接房间 {_roomUrlID} 成功")));

		}

		private void KeepHeartBeat()
		{
			while (IsConnected)
			{
				SendData(0, 16, PROTOCOL_VERSION, 2, 1, "");
				Thread.Sleep(30000);
			}
		}

		private void KeepListen()
		{
			var buffer = new byte[_client.ReceiveBufferSize];
			while (IsConnected)
			{
				try
				{
					_stream.Read(buffer, 0, 4);
					buffer.ToBigEndian(0, 4);
					var len = BitConverter.ToInt32(buffer, 0);
					//                    Console.WriteLine(len);
					_stream.Read(buffer, 4, 4);
					_stream.Read(buffer, 8, 4);
					buffer.ToBigEndian(8, 4);
					var type = BitConverter.ToInt32(buffer, 8);
					_stream.Read(buffer, 12, 4);

					len -= 16;

					//                _stream.Read(buffer, 16, 500);
					//                Console.WriteLine($"Received:");
					//                Console.WriteLine(BitConverter.ToString(buffer));
					//                break;
					if (len != 0)
					{
						_stream.ReadB(buffer, 16, len);
						//                        Console.WriteLine("Received:");
						//                        Console.WriteLine(UsefulBytesToS(buffer, 16, len));

						switch (type)
						{
							case 1:
							case 2:
							case 3:
								{
									Array.Reverse(buffer, 16, 4);
									var viewing = BitConverter.ToUInt32(buffer, 16);
									//                                Console.WriteLine($"Viewing: {viewing}");
									if (viewing >= 3)
									{
										DanmakuList.AddDanmaku(new MessageInfo<MessageType, BMessage>(
											MessageType.OnlineViewerInfo,
											new BOtherMsg($"当前观众：{viewing}")));
									}
									break;
								}
							case 4:
							case 5:
								{
									var danmaku = ParseDanmaku(buffer, 16, len);
									//                                Console.WriteLine(danmaku);
									DanmakuList.AddDanmaku(danmaku);
									break;
								}

							default:
								break;
						}
					}
				}
				catch (Exception)
				{
					continue;
				}
			}
		}

		public void Disconnect()
		{
			if (IsConnected)
			{
				IsConnected = false;
				_client.Close();
				_stream.Close();
			}
		}

		private bool SendData(int length, ushort magic, ushort version,
			int action, int param, byte[] body)
		{
			if (length == 0)
				length = body.Length + 16;

			byte[] u16byte;
			byte[] u32byte;
			var sendbytes = new byte[length];

			u32byte = BitConverter.GetBytes(length);
			if (isLittleEndian)
				Array.Reverse(u32byte);
			Array.Copy(u32byte, 0, sendbytes, 0, 4);

			u16byte = BitConverter.GetBytes(magic);
			if (isLittleEndian)
				Array.Reverse(u16byte);
			Array.Copy(u16byte, 0, sendbytes, 4, 2);

			u16byte = BitConverter.GetBytes(version);
			if (isLittleEndian)
				Array.Reverse(u16byte);
			Array.Copy(u16byte, 0, sendbytes, 6, 2);

			u32byte = BitConverter.GetBytes(action);
			if (isLittleEndian)
				Array.Reverse(u32byte);
			Array.Copy(u32byte, 0, sendbytes, 8, 4);

			u32byte = BitConverter.GetBytes(param);
			if (isLittleEndian)
				Array.Reverse(u32byte);
			Array.Copy(u32byte, 0, sendbytes, 12, 4);

			Array.Copy(body, 0, sendbytes, 16, body.Length);

			//            Console.WriteLine("Sent: ");
			//            Console.WriteLine(ToHexString(sendbytes));
			_stream.Write(sendbytes, 0, sendbytes.Length);
			_stream.Flush();
			return true;
		}

		private bool SendData(int length, ushort magic, ushort version,
			int action, int param, string body)
		{
			var b = Encoding.UTF8.GetBytes(body);
			return SendData(length, magic, version, action, param, b);
		}

		private bool JoinChannel()
		{
			_fake_uid = (long)(100000000000000.0 + 200000000000000.0 * new Random().NextDouble());
			var body = $"{{\"roomid\":{_trueID},\"uid\":{_fake_uid}}}";

			SendData(0, 16, PROTOCOL_VERSION, 7, 1, body);
			return true;
		}

		public static string ToHexString(byte[] bytes)
		{
			var hexString = string.Empty;
			if (bytes != null)
			{
				var strB = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					strB.Append(bytes[i].ToString("X2"));
				}

				hexString = strB.ToString();
			}

			return hexString;
		}

		private static string UsefulBytesToS(byte[] arr, int offset, int count)
		{
			return Encoding.UTF8.GetString(arr, offset, count);
		}

		private static MessageInfo<MessageType, BMessage> ParseDanmaku(byte[] data, int offset, int count)
		{
			var json = Encoding.UTF8.GetString(data, offset, count);
			var jobj = JObject.Parse(json);
			var cmd = jobj["cmd"].Value<string>();
			MessageInfo<MessageType, BMessage> info = null;
			if (cmd == "DANMU_MSG")
			{
				var danmaku = jobj["info"][1].Value<string>();

				var userInfo = jobj["info"][2];
				var username = userInfo[1].Value<string>();
				var isAdmin = userInfo[2].Value<int>() == 1;
				var isVIP = userInfo[3].Value<int>() == 1;
				var isSVIP = userInfo[4].Value<int>() == 1;

				var prefixInfo = jobj["info"][3];
				var badge = (BBadge)null;
				if (prefixInfo.Count() != 0)
				{
					var xunzhang = prefixInfo[1].Value<string>();
					var xunzhangLvl = prefixInfo[0].Value<int>();
					badge = new BBadge(xunzhang, xunzhangLvl);
				}
				var user = new BUser(username, isAdmin, isSVIP, badge);

				info = new MessageInfo<MessageType, BMessage>(MessageType.Danmaku, new BDanmaku(user, danmaku));
			}
			else if (cmd == "SEND_GIFT")
			{
				var giftName = jobj["data"]["giftName"].Value<string>();
				var user = jobj["data"]["uname"].Value<string>();
				var num = jobj["data"]["num"].Value<int>();
				info = new MessageInfo<MessageType, BMessage>(
					MessageType.Gift, new BGift(user, giftName, num));
			}
			else if (cmd == "WELCOME")
			{
				var user = jobj["data"]["uname"].Value<string>();
				info = new MessageInfo<MessageType, BMessage>(MessageType.EnterRoom, new BWelcome(user));
			}

			return info;
		}
	}

	//    struct DataPack
	//    {
	//        public int length;
	//        public ushort magic;
	//        public ushort version;
	//        public int action;
	//        public int param;
	//    }

	public enum LiveState
	{
		Offline,
		Round,
		Live
	}
}