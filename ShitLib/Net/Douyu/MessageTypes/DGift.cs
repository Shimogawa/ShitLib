
namespace ShitLib.Net.Douyu.MessageTypes
{
	public class DGift : DMessage
	{
		public DUser User { get; }

		public int GiftID { get; }

		public string GiftName { get; }

		public int Amount { get; }

		public int Combo { get; }

		public DGift(DUser user, int giftId, string gift, int amount, int hits)
		{
			User = user;
			GiftID = giftId;
			GiftName = gift;
			Amount = amount;
			Combo = hits;

			WholeMessage = $"{user.Username} 送出 {gift} * {amount}";
			if (hits != 1) WholeMessage += $" （{hits}连击！）";
		}

		public DGift(DUser user, int giftID, int amount, int hits) 
			: this(user, giftID, GetGiftName(giftID), amount, hits)
		{
		}

		public static string GetGiftName(int gfid)
		{
			switch (gfid)
			{
				case 191:
					return "100鱼丸";
				case 192:
					return "赞";
				case 193:
					return "弱鸡";
				case 195:
					return "飞机";
				case 519:
					return "呵呵";
				case 520:
					return "稳";
				case 712:
					return "棒棒哒";
				case 713:
					return "辣眼睛";
				case 714:
					return "怂";
				case 750:
					return "办卡";
				case 824:
					return "粉丝荧光棒";
				case 1181:
					return "猛男";
				case 1970:
					return "参份证";
				case 2079:
					return "车队通行证";
				case 2097:
					return "幸运戒指";
				default:
					return gfid.ToString();
			}
		}
	}
}
