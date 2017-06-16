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
        private bool _showComplete = false;
        public bool ShowComplete
        {
            get
            {
                return this._showComplete;
            }
        }

        protected override void InitGroupId ()
        {
            _groupId = (int)EUIGroupType.InGamePopup;
        }

        protected override void InitEventListener ()
        {
            base.InitEventListener ();
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            _timer = 0f;
            _showComplete = false;
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
            if (!_isOpen)
            {
                return;
            }
            base.OnUpdate ();
            _timer += Time.deltaTime;
            if (_timer > _showTime)
            {
                Close ();
                _showComplete = true;
            }
        }
    }
}
