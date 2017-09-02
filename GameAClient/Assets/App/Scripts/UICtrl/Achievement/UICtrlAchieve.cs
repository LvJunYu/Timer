using System.Collections;
using SoyEngine;
using System.Collections.Generic;
using GameA.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameA
{
    /// <summary>
    /// 成就弹窗
    /// </summary>
    [UIAutoSetup]
    public class UICtrlAchieve : UICtrlAnimationBase<UIViewAchieve>
    {
        private Queue<Table_Achievement> _queue = new Queue<Table_Achievement>(5);

        private void OnAchieve(Table_Achievement table_Achievement)
        {
            _queue.Enqueue(table_Achievement);
            if (!_isShowing)
                CoroutineProxy.Instance.StartCoroutine(Show());
        }

        private WaitForSeconds _showTime = new WaitForSeconds(3);
        private bool _isShowing;

        private IEnumerator Show()
        {
            while (_queue.Count > 0 )
            {
                while (_isShowing)
                {
                    yield return null;
                }
                _isShowing = true;
                var table_Achievement = _queue.Dequeue();
                SocialGUIManager.Instance.OpenUI<UICtrlAchieve>();
                _cachedView.NameTxt.text = table_Achievement.Name;
                _cachedView.DescTxt.text = table_Achievement.Description;
                _cachedView.RewardTxt.text = table_Achievement.Reward.ToString();
                yield return _showTime;
                SocialGUIManager.Instance.CloseUI<UICtrlAchieve>();
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlAchieve>();
        }

        protected override void OnCloseAnimationComplete()
        {
            base.OnCloseAnimationComplete();
            _isShowing = false;
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _animationType = EAnimationType.MoveFromUp;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            Messenger<Table_Achievement>.AddListener(EMessengerType.OnAchieve, OnAchieve);
        }
    }
}