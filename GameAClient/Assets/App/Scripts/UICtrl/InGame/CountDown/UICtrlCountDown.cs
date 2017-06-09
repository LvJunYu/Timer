/********************************************************************
** Filename : UICtrlGamePlay  
** Author : ake
** Date : 4/27/2016 8:44:10 PM
** Summary : UICtrlGamePlay  
***********************************************************************/


using System.Diagnostics;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlCountDown : UICtrlInGameBase<UIViewCountDown>
    {
        private float _timer;
        // 界面显示的时间
        private const float _showTime = 1.5f;

        protected override void InitGroupId ()
        {
            _groupId = (int)EUIGroupType.InGamePopup;
        }

        protected override void InitEventListener ()
        {
            base.InitEventListener ();
//            Messenger.AddListener (EMessengerType.OnReady2Play, OnReady2Play);
            Messenger<System.Collections.Generic.List<int>>.AddListener (EMessengerType.OnBoostItemSelectFinish, OnBoostItemSelectFinish);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            _timer = 0f;

            int winConditionCnt = 0;
            if (Game.PlayMode.Instance.SceneState.HasWinCondition (Game.EWinCondition.Arrived)) {
                Sprite sprite = null;
                //GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [0], out sprite);
                //if (sprite != null) {
                //    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                //    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                //}
                winConditionCnt++;
            }

            if (Game.PlayMode.Instance.SceneState.HasWinCondition (Game.EWinCondition.CollectTreasure)) {
                Sprite sprite = null;
                //GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [1], out sprite);
                //if (sprite != null) {
                //    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                //    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                //}
                winConditionCnt++;
            }

            if (Game.PlayMode.Instance.SceneState.HasWinCondition (Game.EWinCondition.KillMonster)) {
                Sprite sprite = null;
                //GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [2], out sprite);
                //if (sprite != null) {
                //    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                //    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                //}
                winConditionCnt++;
            }

            if (Game.PlayMode.Instance.SceneState.HasWinCondition (Game.EWinCondition.TimeLimit)) {
                Sprite sprite = null;
                //GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [3], out sprite);
                //if (sprite != null) {
                //    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                //    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                //}
                winConditionCnt++;
            }

            for (int i = winConditionCnt; i < 4; i++) {
                _cachedView._conditionObjects [i].SetActive (false);
            }

        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            _timer += Time.deltaTime;
            if (_timer > _showTime)
            {
                GameRun.Instance.Playing();
                Close ();
            }
        }

//        private void OnReady2Play ()
//        {
//            // 除了当人模式的普通关卡和挑战关卡需要先选增益道具再展示胜利条件，其他情况下直接展示胜利条件
//            if (EProjectStatus.PS_AdvNormal == GM2DGame.Instance.Project.ProjectStatus ||
//                EProjectStatus.PS_Challenge == GM2DGame.Instance.Project.ProjectStatus
//               )
//                return;
//            SocialGUIManager.Instance.OpenUI<UICtrlCountDown> ();
//        }

        private void OnBoostItemSelectFinish (System.Collections.Generic.List<int> selectedItems)
        {
            UnityEngine.Debug.Log (" GM2DGame.Instance.GameMode.GameRunMode: " + GM2DGame.Instance.GameMode.GameRunMode);
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                GameRun.Instance.Playing();
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlCountDown>();
            }
        }
    }
}
