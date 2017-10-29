/********************************************************************
** Filename : ProjectPlayRecord.cs
** Author : quan
** Date : 1/4/2017 7:30 PM
** Summary : ProjectPlayRecord.cs
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{

	public partial class WorldRankItem
	{
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
