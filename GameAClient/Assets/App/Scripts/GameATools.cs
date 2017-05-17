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
        /// 检查是否有num金币，没有则弹出购买金币对话框
        /// </summary>
        /// <returns><c>true</c>, if gold was checked, <c>false</c> otherwise.</returns>
        /// <param name="num">Number.</param>
        public static bool CheckGold (int num, bool showBuyGold = true) {
            if (LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin >= num) {
                return true;
            } else {
                if (showBuyGold) {
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
        public static bool CheckEnergy (int num, bool showBuyEnergy = true) {
            AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh ();
            if (num > AppData.Instance.AdventureData.UserData.UserEnergyData.Energy) {
                if (showBuyEnergy) {
                    // todo buy energy ui
                }
                return false;
            } else {
                return true;
            }
        }

        public static bool UseEnergy (int num) {
            if (num > AppData.Instance.AdventureData.UserData.UserEnergyData.Energy) {
                return false;
            } else {
                AppData.Instance.AdventureData.UserData.UserEnergyData.Energy -= num;
                return true;
            }
        }
    }
}