using System;
using SoyEngine;
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
                    SocialGUIManager.Instance.OpenUI<UICtrlPurchase> ();
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
                    SocialGUIManager.Instance.OpenUI<UICtrlPurchase> ();
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
                    SocialGUIManager.Instance.OpenUI <UICtrlBuyEnergy> (num - AppData.Instance.AdventureData.UserData.UserEnergyData.Energy);
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

        /// <summary>
        /// 输出年月日 不显示后面的时分秒
        /// </summary>
        /// <param name="timeMillis"></param>
        /// <param name="DateType"></param>
        /// <returns></returns>
        public static string GetYearMonthDayHourMinuteSecondByMilli(long timeMillis, int DateType)
        {
            int timezone = 8; // 时区  
            long totalSeconds = timeMillis/1000;
            totalSeconds += 60*60*timezone;
            int second = (int) (totalSeconds%60); // 秒  
            long totalMinutes = totalSeconds/60;
            int minute = (int) (totalMinutes%60); // 分  
            long totalHours = totalMinutes/60;
            int hour = (int) (totalHours%24); // 时  
            int totalDays = (int) (totalHours/24);
            int _year = 1970;
            int year = _year + totalDays/366;
            int month = 1;
            int day = 1;
            int diffDays;
            bool leapYear;
            while (true)
            {
                int diff = (year - _year)*365;
                diff += (year - 1)/4 - (_year - 1)/4;
                diff -= ((year - 1)/100 - (_year - 1)/100);
                diff += (year - 1)/400 - (_year - 1)/400;
                diffDays = totalDays - diff;
                leapYear = (year%4 == 0) && (year%100 != 0) || (year%400 == 0);
                if (!leapYear && diffDays < 365 || leapYear && diffDays < 366)
                {
                    break;
                }
                else
                {
                    year++;
                }
            }
            int[] monthDays;
            if (diffDays >= 59 && leapYear)
            {
                monthDays = new int[] {-1, 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335};
            }
            else
            {
                monthDays = new int[] {-1, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334};
            }
            for (int i = monthDays.Length - 1; i >= 1; i--)
            {
                if (diffDays >= monthDays[i])
                {
                    month = i;
                    day = diffDays - monthDays[i] + 1;
                    break;
                }
            }
            switch (DateType)
            {
                case 1:
                    return year + "." + month + "." + day;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 输入秒 输出时分秒 时为0则不显示时 时分为0则不显示时分
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string SecondToHour(float time)
        {
            string str = "";
            int hour = 0;
            int minute = 0;
            float second = time;
            //second = Convert.ToInt32(time);
            //float second1 = (float) Math.Round(time*100);
            //Debug.Log("____second1" + second1);
            //second =
            //Debug.Log("____second1" + second);


            if (second > 60)
            {
                minute = (int) second/60;
                second = second%60;
            }
            if (minute > 60)
            {
                hour = minute/60;
                minute = minute%60;
            }

            if (hour > 1)
            {
                return (minute + "′"
                        + (float) (Math.Round(second*100)/100) + "″");
            }
            else if (minute > 1)
            {
                return (minute + "′"
                        + (float) (Math.Round(second*100)/100) + "″");
            }
            else
            {
                return ((float) (Math.Round(second*100)/100) + "″");
            }
        }

        public static string GetLevelString(int level)
        {
            return string.Format("Lv.{0}", level);
        }

        public static string GetCompleteRateString(float rate)
        {
            return "" + Mathf.CeilToInt(rate*1000)/10f + " %";
        }
    }
}