/********************************************************************
** Filename : UICtrlGamePlay  
** Author : ake
** Date : 4/27/2016 8:44:10 PM
** Summary : UICtrlGamePlay  
***********************************************************************/


using System.Diagnostics;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlCountDown : UICtrlGenericBase<UIViewCountDown>
    {
        private float _timer;
        // 界面显示的时间
        private const float _showTime = 1.5f;
        // 界面关闭的回调函数
        private System.Action _callback = null;

        protected override void InitGroupId ()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            _timer = 0f;

            int winConditionCnt = 0;
            if (PlayMode.Instance.SceneState.HasWinCondition (EWinCondition.Arrived)) {
                Sprite sprite = null;
                GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [0], out sprite);
                if (sprite != null) {
                    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                }
                winConditionCnt++;
            }

            if (PlayMode.Instance.SceneState.HasWinCondition (EWinCondition.CollectTreasure)) {
                Sprite sprite = null;
                GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [1], out sprite);
                if (sprite != null) {
                    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                }
                winConditionCnt++;
            }

            if (PlayMode.Instance.SceneState.HasWinCondition (EWinCondition.KillMonster)) {
                Sprite sprite = null;
                GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [2], out sprite);
                if (sprite != null) {
                    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                }
                winConditionCnt++;
            }

            if (PlayMode.Instance.SceneState.HasWinCondition (EWinCondition.TimeLimit)) {
                Sprite sprite = null;
                GameResourceManager.Instance.TryGetSpriteByName (_cachedView._spriteNames [3], out sprite);
                if (sprite != null) {
                    _cachedView._smallImages [winConditionCnt].sprite = sprite;
                    _cachedView._bigImages [winConditionCnt].sprite = sprite;
                }
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
            if (_timer > _showTime) {
                Close ();
                if (_callback != null) {
                    _callback ();
                }
            }
        }

        public void SetCallback (System.Action callback)
        {
            _callback = callback;
        }
    }
}
