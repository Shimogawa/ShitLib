using System.Text;

namespace ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes
{
    public class BDanmaku : BMessage
    {
        /// <summary>
        /// Always have 3 elements.
        /// [0]: is room admin (null if not)
        /// [1]: is svip (null if not)
        /// [2]: xunzhang name and level (null if not existing)
        /// </summary>
        public string[] Prefix { get; }

        public string Danmaku { get; }

        public BDanmaku(string[] prefix, string username, string danmaku)
        {
            Prefix = prefix;
            Username = username;
            Danmaku = danmaku;

            var sb = new StringBuilder();
            foreach (var s in prefix)
            {
                if (s != null)
                    sb.Append(s);
            }

            sb.Append($" {username} 说： {danmaku}");
            WholeMessage = sb.ToString();
        }
    }
}
