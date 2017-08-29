//  | 获取冒险关卡用户数据

namespace GameA
{
	public partial class AdventureUserLevelData
	{
		public int GotStarCnt {
			get { return (_star1Flag ? 1 : 0) + (_star2Flag ? 1 : 0) + (Star3Flag ? 1 : 0); }
		}
	}
}