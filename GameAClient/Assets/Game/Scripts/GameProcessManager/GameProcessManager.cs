using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;

namespace GameA.Game
{
    /// <summary>
    /// 游戏进程管理器
    /// </summary>
    public class GameProcessManager : Singleton<GameProcessManager>
    {
        #region fields
        private List<UnlockItem> _itemsToUnlock = new List<UnlockItem>();
        private Queue<UnlockProcess> _wait2ShowProcessQueue = new Queue<UnlockProcess>();
        private UnlockProcess _currentShowingUnlockProcess;
        #endregion

        #region properties
        #endregion

        #region methods
        public void Init ()
        {
            Messenger.AddListener (EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
            InitItemsToUnlock ();
            _wait2ShowProcessQueue.Clear ();
            _currentShowingUnlockProcess = null;

            RefreshHomeUIUnlock ();
        }

        public void Clear ()
        {
            Messenger.RemoveListener (EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
        }

        /// <summary>
        /// 初始化可能解锁的项目
        /// </summary>
        private void InitItemsToUnlock ()
        {
            AdvGameProcess currentProcess = 
                new AdvGameProcess (
                    AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteSection,
                    AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteLevel
                );
//            Debug.Log ("Current process: " + currentProcess);
            _itemsToUnlock.Clear ();
            var tableDic = TableManager.Instance.Table_ProgressUnlockDic;
            var itor = tableDic.GetEnumerator ();
            while (itor.MoveNext())
            {
                AdvGameProcess process = new AdvGameProcess (itor.Current.Value.Chapter, itor.Current.Value.Level);
                // todo，和当前相等的也要加进去，然后在进入家园时判断，如果这个流程还没有进行，则进行流程
                if (process > currentProcess) 
                {
                    UnlockItem item = new UnlockItem ();
                    item.Table = itor.Current.Value;
                    item.Process = process;
                    _itemsToUnlock.Add (item);
//                    Debug.Log ("Add new process: " + process + " id: " + itor.Current.Value.Id);
                }
            }
            _itemsToUnlock = _itemsToUnlock.OrderBy (i => i.Process).ToList();
        }

        private void OnChangeToAppMode ()
        {
            AdvGameProcess currentProcess = 
                new AdvGameProcess (
                    AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteSection,
                    AppData.Instance.AdventureData.UserData.AdventureUserProgress.CompleteLevel
                );
//            Debug.Log ("__________________OnChangeToAppMode");
//            Debug.Log ("current: " + currentProcess);
            bool newItemUnlocked = false;
            for (int i = _itemsToUnlock.Count - 1; i >= 0 ; i--) {
//                Debug.Log ("_itemsToUnlock [i].Process: " + _itemsToUnlock [i].Process);
                if (_itemsToUnlock [i].Process <= currentProcess) {
//                    Debug.Log ("Try to pla  y process: " + _itemsToUnlock [i].Process);
                    var newProcess = UnlockProcessCreator.GetUnlockProcess (_itemsToUnlock [i]);
                    if (null != newProcess) _wait2ShowProcessQueue.Enqueue (newProcess);
                    newItemUnlocked = true;
                    _itemsToUnlock.RemoveAt (i);
                }
            }

            // todo 流程的进行
            if (null != _currentShowingUnlockProcess) {
                if (!_currentShowingUnlockProcess.IsRunning) {
                    _currentShowingUnlockProcess = null;
                }
            }
            if (_wait2ShowProcessQueue.Count != 0) {
                _currentShowingUnlockProcess = _wait2ShowProcessQueue.Dequeue ();
            }
            if (null != _currentShowingUnlockProcess) {
                _currentShowingUnlockProcess.Start ();
            }

            if (newItemUnlocked) {
                RefreshHomeUIUnlock ();
            }
        }

        public void RefreshHomeUIUnlock() 
        {
            var uiTaskBar = SocialGUIManager.Instance.GetUI<UICtrlTaskbar> ();
            if (null != uiTaskBar) {
                uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_Lottery, true);
                uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_FashionShop, true);
                uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_Workshop, true);
                uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_World, true);
                // todo 改表结构，增加枚举列，不能判断id
                for (int i = 0; i < _itemsToUnlock.Count; i++) {
                    if (_itemsToUnlock [i].Table.Id == 4) {
                        uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_Lottery, false);
                    } else if (_itemsToUnlock [i].Table.Id == 5) {
                        uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_FashionShop, false);
                    } else if (_itemsToUnlock [i].Table.Id == 6) {
                        uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_Workshop, false);
                    } else if (_itemsToUnlock [i].Table.Id == 7) {
//                    uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_Lottery, false);
                    } else if (_itemsToUnlock [i].Table.Id == 8) {
                        uiTaskBar.SetLock (UICtrlTaskbar.UIFunction.UI_World, false);
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 冒险模式进程
    /// </summary>
    public class AdvGameProcess : IComparable<AdvGameProcess>
    {
        public int Chapter;
        public int Level;

        public AdvGameProcess (int chapter, int level)
        {
            Chapter = chapter;
            Level = level;
        }

        public static bool operator > (AdvGameProcess a, AdvGameProcess b)
        {
            if (a.Chapter > b.Chapter) return true;
            if (a.Chapter < b.Chapter) return false;
            if (a.Level > b.Level) return true;
            if (a.Level < b.Level) return false;
            return false;
        }
        public static bool operator < (AdvGameProcess a, AdvGameProcess b)
        {
            if (a.Chapter > b.Chapter) return false;
            if (a.Chapter < b.Chapter) return true;
            if (a.Level > b.Level) return false;
            if (a.Level < b.Level) return true;
            return false;
        }
        public static bool operator >= (AdvGameProcess a, AdvGameProcess b)
        {
            if (a.Chapter > b.Chapter) return true;
            if (a.Chapter < b.Chapter) return false;
            if (a.Level > b.Level) return true;
            if (a.Level < b.Level) return false;
            return true;
        }
        public static bool operator <= (AdvGameProcess a, AdvGameProcess b)
        {
            if (a.Chapter > b.Chapter) return false;
            if (a.Chapter < b.Chapter) return true;
            if (a.Level > b.Level) return false;
            if (a.Level < b.Level) return true;
            return true;
        }
        public static bool operator == (AdvGameProcess a, AdvGameProcess b)
        {
            return a.Chapter == b.Chapter && a.Level == b.Level;
        }
        public static bool operator != (AdvGameProcess a, AdvGameProcess b)
        {
            return a.Chapter != b.Chapter || a.Level != b.Level;
        }
        public int CompareTo(AdvGameProcess that)
        {
            if (this.Chapter > that.Chapter) return 1;
            if (this.Chapter < that.Chapter) return -1;
            if (this.Level > that.Level) return 1;
            if (this.Level < that.Level) return -1;
            return 0;
        }
        public override string ToString ()
        {
            return string.Format ("[AdvGameProcess]: Chapter {0}, Level {1}", this.Chapter, this.Level);
        }
    }


    /// <summary>
    /// 解锁项目
    /// </summary>
    public class UnlockItem
    {
        public Table_ProgressUnlock Table;
        public AdvGameProcess Process;
    }

    /// <summary>
    /// 解锁流程
    /// </summary>
    public class UnlockProcess
    {
        private UnlockItem _unlockItem;
        /// <summary>
        /// 动作列表
        /// </summary>
        private List<ProcessActionBase> _actions = new List<ProcessActionBase>();

        private bool _run = false;

        public bool IsRunning
        {
            get { return _run; }
        }

        public UnlockProcess (UnlockItem item)
        {
            _unlockItem = item;
            _run = false;
        }

        public void Start ()
        {
            _run = true;
            Next ();
        }

        public void Next ()
        {
            if (_actions.Count > 0) {
                var action = _actions [0];
                action.DoAction (Next);
                _actions.RemoveAt (0);
            } else {
                _run = false;
            }
        }

        public void AddAction (ProcessActionBase action)
        {
            _actions.Add (action);
        }
    }

    /// <summary>
    /// 解锁动作基类
    /// </summary>
    public class ProcessActionBase
    {
        protected Action _finishCB;
        public virtual void DoAction (Action finishCB) {
            _finishCB = finishCB;
        }
    }

    public class ProcessActionShowRewardWindow : ProcessActionBase
    {
        // 是否显示系统解锁界面（否则是能力解锁界面）
        private bool _isSystem;
        private string _title;
        private string _icon;
        public override void DoAction (Action finishCB)
        {
            base.DoAction (finishCB);
            if (_isSystem) {
                SocialGUIManager.ShowUnlockSystem (_title, _icon, _finishCB);
            } else {
                SocialGUIManager.ShowUnlockAbility (_title, _icon, _finishCB);
            }
        }

        public ProcessActionShowRewardWindow (bool isSystem, string title, string icon)
        {
            _isSystem = _isSystem;
            _title = title;
            _icon = icon;
        }
    }

    /// <summary>
    /// 组装流程
    /// </summary>
    public static class UnlockProcessCreator
    {
        public static UnlockProcess GetUnlockProcess (UnlockItem unlockItem)
        {
            UnlockProcess process = new UnlockProcess (unlockItem);
            if (unlockItem.Table.Id == 1) {
                ProcessActionShowRewardWindow action1 = new ProcessActionShowRewardWindow (false, unlockItem.Table.Desc, unlockItem.Table.Icon);
                process.AddAction (action1);
            } else if (unlockItem.Table.Id == 2) {
                ProcessActionShowRewardWindow action1 = new ProcessActionShowRewardWindow (false, unlockItem.Table.Desc, unlockItem.Table.Icon);
                process.AddAction (action1);
            } else if (unlockItem.Table.Id == 3) {
                ProcessActionShowRewardWindow action1 = new ProcessActionShowRewardWindow (false, unlockItem.Table.Desc, unlockItem.Table.Icon);
                process.AddAction (action1);
            } else if (unlockItem.Table.Id == 4) {
                ProcessActionShowRewardWindow action1 = new ProcessActionShowRewardWindow (true, unlockItem.Table.Desc, unlockItem.Table.Icon);
                process.AddAction (action1);
            } else if (unlockItem.Table.Id == 5) {
                ProcessActionShowRewardWindow action1 = new ProcessActionShowRewardWindow (true, unlockItem.Table.Desc, unlockItem.Table.Icon);
                process.AddAction (action1);
            } else {
                return null;
            }
            return process;
        }
    }
}