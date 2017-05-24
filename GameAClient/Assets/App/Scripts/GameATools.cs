using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace GameA {
    /// <summary>
    /// 提供了一些常用的工具
    /// </summary>
    public class GameATools {
        /// <summary>
        /// 刷新本地脏数据（金币、钻石、体力）的最小间隔时间
        /// </summary>
        private static long _updateLocalDirtyValueInterval = 30000;

        #region check gde
        /// <summary>
        /// 检查是否有num金币，没有则弹出购买金币对话框
        /// </summary>
        /// <returns><c>true</c>, if gold was checked, <c>false</c> otherwise.</returns>
        /// <param name="num">Number.</param>
        public static bool CheckGold (int num, bool showBuy = true) {
            if (LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin >= num) {
                return true;
            } else {
                if (showBuy) {
                    // todo buy gold ui
                }
                return false;
            }
        }
        /// <summary>
        /// 检查是否有num钻石，没有则弹出购买钻石对话框
        /// </summary>
        /// <returns><c>true</c>, if gold was checked, <c>false</c> otherwise.</returns>
        /// <param name="num">Number.</param>
        public static bool CheckDiamond (int num, bool showBuy = true) {
            if (LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond >= num) {
                return true;
            } else {
                if (showBuy) {
                    // todo buy gold ui
                }
                return false;
            }
        }
        /// <summary>
        /// 检查是否有num体力，没有则弹出购买体力对话框
        /// </summary>
        /// <returns><c>true</c>, if energy was checked, <c>false</c> otherwise.</returns>
        /// <param name="num">Number.</param>
        /// <param name="showBuyEnergy">If set to <c>true</c> show buy energy.</param>
        public static bool CheckEnergy (int num, bool showBuy = true) {
            AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh ();
            if (num > AppData.Instance.AdventureData.UserData.UserEnergyData.Energy) {
                if (showBuy) {
                    // todo buy energy ui
                    SocialGUIManager.Instance.OpenPopupUI<UICtrlBuyEnergy> (num - AppData.Instance.AdventureData.UserData.UserEnergyData.Energy);
                }
                return false;
            } else {
                return true;
            }
        }
        #endregion

        #region use gde
        public static bool LocalUseEnergy (int num) {
            if (num > AppData.Instance.AdventureData.UserData.UserEnergyData.Energy) {
                return false;
            } else {
                AppData.Instance.AdventureData.UserData.UserEnergyData.Energy -= num;
                if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                    AppData.Instance.AdventureData.UserData.UserEnergyData.FirstDirtyTime >
                    _updateLocalDirtyValueInterval) {
                    AppData.Instance.AdventureData.UserData.UserEnergyData.Request (
                        LocalUser.Instance.UserGuid,
                        null,
                        code => {
                            // todo error handle
                        }
                    );
                }
                //Messenger.Broadcast (EMessengerType.OnEnergyChanged);
                AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh ();
                return true;
            }
        }
        public static bool LocalUseGold (int num) {
            if (num > LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin) {
                return false;
            } else {
                LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin -= num;
                if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                    LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                    _updateLocalDirtyValueInterval) {
                    LocalUser.Instance.User.UserInfoSimple.LevelData.Request (
                        LocalUser.Instance.UserGuid,
                        null,
                        code => {
                            // todo error handle
                        }
                    );
                }
                Messenger.Broadcast (EMessengerType.OnGoldChanged);
                return true;
            }
        }
        public static bool LocalUseDiamond (int num) {
            if (num > LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond) {
                return false;
            } else {
                LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond -= num;
                if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                    LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                    _updateLocalDirtyValueInterval) {
                    LocalUser.Instance.User.UserInfoSimple.LevelData.Request (
                        LocalUser.Instance.UserGuid,
                        null,
                        code => {
                            // todo error handle
                        }
                    );
                }
                Messenger.Broadcast (EMessengerType.OnDiamondChanged);
                return true;
            }
        }
        #endregion

        #region add gde
        public static void LocalAddEnergy (int num) {
            AppData.Instance.AdventureData.UserData.UserEnergyData.Energy += num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                AppData.Instance.AdventureData.UserData.UserEnergyData.FirstDirtyTime >
                _updateLocalDirtyValueInterval) {
                AppData.Instance.AdventureData.UserData.UserEnergyData.Request (
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            //Messenger.Broadcast (EMessengerType.OnEnergyChanged);
            AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh ();
        }
        public static void LocalAddGold (int num) {
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                _updateLocalDirtyValueInterval) {
                LocalUser.Instance.User.UserInfoSimple.LevelData.Request (
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            Messenger.Broadcast (EMessengerType.OnGoldChanged);
        }
        public static void LocalAddDiamond (int num) {
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                _updateLocalDirtyValueInterval) {
                LocalUser.Instance.User.UserInfoSimple.LevelData.Request (
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            Messenger.Broadcast (EMessengerType.OnDiamondChanged);
        }

        public static void LocalAddPlayerExp(int num)
        {
            LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerExp += num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                _updateLocalDirtyValueInterval)
            {
                LocalUser.Instance.User.UserInfoSimple.LevelData.Request(
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            Messenger.Broadcast(EMessengerType.OnDiamondChanged);
        }

        public static void LocalAddCreatorExp(int num)
        {
            LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorExp += num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                _updateLocalDirtyValueInterval)
            {
                LocalUser.Instance.User.UserInfoSimple.LevelData.Request(
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            Messenger.Broadcast(EMessengerType.OnDiamondChanged);
        }
        #endregion

        #region set gde
        public static void LocalSetEnergy (int num) {
            AppData.Instance.AdventureData.UserData.UserEnergyData.Energy = num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                AppData.Instance.AdventureData.UserData.UserEnergyData.FirstDirtyTime >
                _updateLocalDirtyValueInterval) {
                AppData.Instance.AdventureData.UserData.UserEnergyData.Request (
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            //Messenger.Broadcast (EMessengerType.OnEnergyChanged);
            AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh ();
        }
        public static void LocalSetGold (int num) {
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin = num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                _updateLocalDirtyValueInterval) {
                LocalUser.Instance.User.UserInfoSimple.LevelData.Request (
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            Messenger.Broadcast (EMessengerType.OnGoldChanged);
        }
        public static void LocalSetDiamond (int num) {
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond = num;
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() -
                LocalUser.Instance.User.UserInfoSimple.LevelData.FirstDirtyTime >
                _updateLocalDirtyValueInterval) {
                LocalUser.Instance.User.UserInfoSimple.LevelData.Request (
                    LocalUser.Instance.UserGuid,
                    null,
                    code => {
                        // todo error handle
                    }
                );
            }
            Messenger.Broadcast (EMessengerType.OnDiamondChanged);
        }
        #endregion
    }
}