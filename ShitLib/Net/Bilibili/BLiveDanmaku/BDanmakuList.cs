using System.Collections.Generic;

namespace ShitLib.Net.Bilibili.BLiveDanmaku
{
    public class BDanmakuList
    {
        private LinkedList<BMessageInfo> danmakuList = new LinkedList<BMessageInfo>();

        public BDanmakuList()
        {
            
        }

        public void AddDanmaku(BMessageInfo danmaku)
        {
            danmakuList.AddLast(danmaku);
        }

        public IEnumerable<BMessageInfo> KeepGetting()
        {
            BMessageInfo temp;
            while (true)
            {
                if (!IsEmpty())
                {
                    temp = danmakuList.First.Value;
                    danmakuList.RemoveFirst();
                    yield return temp;
                }
            }
        }

        public BMessageInfo GetFirst()
        {
            if (IsEmpty()) return null;
            var temp = danmakuList.First.Value;
            danmakuList.RemoveFirst();
            return temp;
        }

        public bool IsEmpty()
        {
            return danmakuList.Count == 0;
        }
    }
}