﻿// 获取冒险章节用户数据 | 获取冒险章节用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
	public partial class AdventureUserSection : SyncronisticData {
		public int GotStarCnt {
			get {
				int cnt = 0;
				for (int i = 0, n = _normalLevelUserDataList.Count; i < n; i++) {
					cnt += _normalLevelUserDataList [i].SimpleData.GotStarCnt;
				}
				return cnt;
			}
		}

		public AdventureUserLevelDataDetail LocalAddLevelData (EAdventureProjectType type) {
			AdventureUserLevelDataDetail newLevelData = new AdventureUserLevelDataDetail ();
			if (type == EAdventureProjectType.APT_Bonus) {
				_bonusLevelUserDataList.Add (newLevelData);
			} else {
				
				_normalLevelUserDataList.Add (newLevelData);
			}
			return newLevelData;
		}
	}
}