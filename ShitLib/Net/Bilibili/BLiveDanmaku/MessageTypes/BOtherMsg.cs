namespace ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes
{
    public class BOtherMsg : BMessage
    {
        public BOtherMsg(string msg)
        {
            Username = null;
            WholeMessage = msg;
        }
    }
}
