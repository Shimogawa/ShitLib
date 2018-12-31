using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitLib.Net.Douyu.MessageTypes
{
	public class DMessage
	{
		public string Username { get; internal set; }

		public string WholeMessage { get; internal set; }

		public DMessage() { }

		public DMessage(string msg, string username = null)
		{
			WholeMessage = msg;
			Username = username;
		}

		public override string ToString()
		{
			return WholeMessage;
		}
	}
}
