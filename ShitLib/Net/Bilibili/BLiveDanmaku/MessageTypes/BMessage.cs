using ShitLib.Net.MessageTypes;

namespace ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes
{
    public class BMessage : Message
    {
	    public BMessage() { }

	    public BMessage(string msg, string username = null)
	    {
		    WholeMessage = msg;
		    Username = username;
	    }
	}
}