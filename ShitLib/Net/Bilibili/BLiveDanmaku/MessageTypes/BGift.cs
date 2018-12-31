namespace ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes
{
    public class BGift : BMessage
    {
        public int Amount { get; }

        public string GiftName { get; }

        public BGift(string username, string giftName, int amount)
        {
            Username = username;
            GiftName = giftName;
            Amount = amount;

            WholeMessage = $"{username} 送出 {giftName} * {amount}";
        }
    }
}
