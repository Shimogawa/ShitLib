using ShitLib.Net.Bilibili.BLiveDanmaku.MessageTypes;

namespace ShitLib.Net.Bilibili.BLiveDanmaku
{
    public class BMessageInfo
    {
        public BMessageType MessageType { get; }
        
        public BMessage Message { get; }
        
        public BMessageInfo(BMessageType type, BMessage message)
        {
            MessageType = type;
            Message = message;
        }
    }

    public enum BMessageType : byte
    {
        Danmaku,
        Gift,
        EnterRoom,
        OnlineViewerInfo,
        Log,
        SysMsg,
        Other
    }
}