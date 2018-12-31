namespace ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes
{
    public class BMessage
    {
        public string Username { get; internal set; }
        
        public string WholeMessage { get; internal set; }

        public BMessage() { }

        public BMessage(string msg, string username = null)
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