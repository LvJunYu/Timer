
using SoyEngine.Proto;

namespace GameA
{

	public partial class WorldRankItem
	{
		private UserInfoDetail _userInfoDetail;

		public UserInfoDetail UserInfoDetail
		{
			get { return _userInfoDetail; }
		}


		protected override void OnSyncPartial(Msg_WorldRankItem msg)
		{
			base.OnSyncPartial(msg);
			_userInfoDetail = UserManager.Instance.UpdateData(msg.UserInfo);
		}

		public class WorldRankHolder
		{
			private WorldRankItem _record;
			private int _rank;

			public WorldRankItem Record
			{
				get { return _record; }
			}

			public int Rank
			{
				get { return _rank; }
			}

//        public RecordRankHolder(Msg_SC_DAT_Record msg, Project project, int rank)
//        {
//            _record = RecordManager.Instance.OnSync(msg, project, false);
//            _rank = rank;
//        }

			public WorldRankHolder(WorldRankItem record, int rank)
			{
				_record = record;
				_rank = rank;
			}
		}
	}
}
