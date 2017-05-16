// 用户匹配改造数据 | 用户匹配改造数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MatchUserData : SyncronisticData {
        public enum EChallengeState {
            None,
            WaitForChance,
            ChanceReady,
            Selecting,
            Challenging,
            Max,
        }
        #region 字段
//        // 默认最大体力点
//        private const int DefaultEnergyCapacity = 30;
        // 默认体力增长时间／每点
        private const int DefaultChallengeGenerateTime = 300;


        #endregion

        #region 属性
        /// <summary>
        /// 下一次自动增长挑战机会的时间
        /// </summary>
        /// <returns>The generate time.</returns>
        public long NextChallengeChanceGenerateTime {
            get {
                if (_leftChallengeCount >= _challengeCapacity)
                    return long.MaxValue;
                return _leftChallengeCountRefreshTime + 1000 * DefaultChallengeGenerateTime;
            }
        }

        /// <summary>
        /// 下一次自动增长改造机会的时间
        /// </summary>
        /// <returns>The generate time.</returns>
        public long NextModifyChanceGenerateTime {
            get {
                if (_curReformState == (int)EReformState.RS_WaitForChance) {
                    return _curPublishTime + 1000 * _reformIntervalSeconds;
                } else {
                    return long.MaxValue;
                }
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获得特定id的可添加地块数量
        /// </summary>
        /// <returns>The can add unit number of identifier.</returns>
        /// <param name="unitId">Unit identifier.</param>
        public int GetCanAddUnitNumOfId (int unitId) {
            if (null == _unitData)
                return 0;
            for (int i = 0; i < _unitData.ItemList.Count; i++) {
                if (_unitData.ItemList [i].UnitId == unitId) {
                    return _unitData.ItemList [i].UnitCount;
                }
            }
            return 0;
        }

        /// <summary>
        /// 客户端刷新当前挑战和改造倒计时
        /// </summary>
        public void LocalRefresh () {
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            // challenge



            // modify



//            if (_energy >= _energyCapacity) {
//                EnergyLastRefreshTime = now;
//                return;
//            }
//            long passedTime = now - EnergyLastRefreshTime;
//            int generatedEnergy = (int)(passedTime / 1000 / DefaultEnergyGenerateTime);
//            if (generatedEnergy > 0) {
//                EnergyLastRefreshTime += generatedEnergy * 1000 * DefaultEnergyCapacity;
//                Energy += generatedEnergy;
//                if (Energy >= _energyCapacity) {
//                    EnergyLastRefreshTime = now;
//                    Energy = _energyCapacity;
//                }
//            }
        }
        /// <summary>
        /// 当天挑战状态
        /// </summary>
        /// <returns>The challenge state.</returns>
        public EChallengeState CurrentChallengeState () {
            if (!_inited)
                return EChallengeState.None;

            if (_leftChallengeCount <= 0)
                return EChallengeState.WaitForChance;
            else {
                if (!_easyChallengeProjectData.IsInited &&
                    !_mediumChallengeProjectData.IsInited &&
                    !_difficultChallengeProjectData.IsInited &&
                    !_randomChallengeProjectData.IsInited) {
                    return EChallengeState.ChanceReady;
                } else {                    
                    if (_curSelectedChallengeType == (int)EChallengeProjectType.CPT_None) {
                        return EChallengeState.Selecting;
                    } else {
                        return EChallengeState.Challenging;
                    }
                }
            }
        }

        /// <summary>
        /// 请求放弃挑战成功后的本地数据刷新
        /// </summary>
        public void OnAbandomChallengeSuccess () {
            CurSelectedChallengeType = (int)EChallengeProjectType.CPT_None;
            EasyChallengeProjectData = new Project();
            MediumChallengeProjectData = new Project();
            DifficultChallengeProjectData = new Project();
            RandomChallengeProjectData = new Project();
        }
        #endregion
    }
}