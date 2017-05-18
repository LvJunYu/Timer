// 获取体力数据 | 获取体力数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
	public partial class UserEnergy : SyncronisticData {
		// 默认最大体力点
		private const int DefaultEnergyCapacity = 30;
		// 默认体力增长时间／每点
		private const int DefaultEnergyGenerateTime = 300;
		/// <summary>
		/// 下一次自动增长体力点的时间
		/// </summary>
		/// <returns>The generate time.</returns>
		public long NextGenerateTime {
			get {
				if (_energy >= _energyCapacity)
					return long.MaxValue;
				return _energyLastRefreshTime + 1000 * DefaultEnergyGenerateTime;
			}
		}

		/// <summary>
		/// 客户端刷新当前体力信息
		/// </summary>
		public void LocalRefresh (bool broadcastMsg = true) {
			long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
			if (_energy >= _energyCapacity) {
				EnergyLastRefreshTime = now;
                if (broadcastMsg)
                    Messenger.Broadcast (EMessengerType.OnEnergyChanged);
				return;
			}
			long passedTime = now - EnergyLastRefreshTime;
			int generatedEnergy = (int)(passedTime / 1000 / DefaultEnergyGenerateTime);
			if (generatedEnergy > 0) {
				EnergyLastRefreshTime += generatedEnergy * 1000 * DefaultEnergyCapacity;
				Energy += generatedEnergy;
				if (Energy >= _energyCapacity) {
					EnergyLastRefreshTime = now;
					Energy = _energyCapacity;
				}
            }
            if (broadcastMsg)
                Messenger.Broadcast (EMessengerType.OnEnergyChanged);
		}

        protected override void OnSyncPartial ()
        {
            base.OnSyncPartial ();
            Messenger.Broadcast (EMessengerType.OnEnergyChanged);
        }
	}
}