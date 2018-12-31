namespace ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes
{
    public class BWelcome : BMessage
    {
        public BWelcome(string username)
        {
            Username = username;
            WholeMessage = $"{username} 进入了直播间。";
        }
    }
}
