using System.Diagnostics;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using DG.Tweening;

namespace GameA.Game
{
    public partial class GuideUI : MonoBehaviour
    {
        private string _text;

        private Callback _maskAnimCB;
        private Callback _hideTextCB;
        private Callback _qiePingCB;
        private Callback _showRewardCB;

        private bool _textShow;

        public void PlayExitAnim()
        {

        }
        public void SetText(string text)
        {
            if (_textShow) return;
            _textShow = true;
            _text = text;
            _animation.Play("UIGuideTextOpen");
            DictionaryTools.SetContentText(_label, string.Empty);
        }
        public void ChangeText(string text)
        {
            if (!_textShow) {
                SetText(text);
                return;
            }
            _text = text;
            _animation.Play("UIGuideTextNext");
        }
        public void HideText (Callback cb = null) {
            if (!_textShow) return;
            _textShow = false;
            _animation.Play("UIGuideTextClose");
            _hideTextCB = cb;
        }

        public void ShowReward (Callback cb) {
            _animation.Play("UIGuideTextReward");
            _showRewardCB = cb;
        }

        public void HideReward () {
            _reward.SetActiveEx(false);
        }

		public void SetMask (Guide.EMaskType maskType, Callback cb = null) {
			if (maskType == Guide.EMaskType.None) {
				_mask.SetActiveEx(false);
                _rightBtnPos.SetActiveEx(false);
                _jumpBtnPos.SetActiveEx(false);
                _attackBtnPos.SetActiveEx(false);
                return;
			}
            
            if (maskType == Guide.EMaskType.RightButton) {
                _rightBtnPos.SetActiveEx(true);
				_mask.SetParent(_rightBtnPos, false);
			} else if (maskType == Guide.EMaskType.JumpButton) {
                _jumpBtnPos.SetActiveEx(true);
                _mask.SetParent(_jumpBtnPos, false);
            } else if (maskType == Guide.EMaskType.AttackButton) {
                _attackBtnPos.SetActiveEx(true);
                _mask.SetParent(_attackBtnPos, false);
            }            
            _mask.SetActiveEx(true);
            _mask.localScale = Vector3.one * 100;
            var tweener =_mask.DOScale(18f, 0.5f);
            if (cb != null) {
                tweener.OnComplete(()=>{cb();});
            }
		}

        public void QiePingIn (Callback cb) {
            _qiePingCB = cb;
            _animation.Play("UIGuideTextQiePing1In");
        }

        public void QiePingOut (Callback cb) {
            _qiePingCB = cb;
            _animation.Play("UIGuideTextQiePing1Out");
        }

        private void AnimCB(string animName)
        {
            if (string.Compare(animName, "open") == 0 ||
            string.Compare(animName, "next") == 0)
            {
                if (_text != null)
                {
                    DictionaryTools.SetContentText(_label, _text);
                    _text = null;
                }
            }
            else if (string.Compare(animName, "close") == 0)
            {
                // GameObject.Destroy(this.gameObject);
                if (_hideTextCB != null) {
                    _hideTextCB();
                }
            }
            else if (string.Compare(animName, "reward") == 0) {
                if (_showRewardCB != null) {
                    _showRewardCB();
                }
            }
            else if (string.Compare(animName, "qiePingIn") == 0)
            {
                // GameObject.Destroy(this.gameObject);
                if (_qiePingCB != null) {
                    _qiePingCB();
                }
            }
            else if (string.Compare(animName, "qiePingOut") == 0)
            {
                // GameObject.Destroy(this.gameObject);
                if (_qiePingCB != null) {
                    _qiePingCB();
                }
            }
        }

        void Awake ()
        {
            // base.OnOpen(parameter);
            
            _textShow = false;
            _animCB += AnimCB;
        }

        // protected override void OnClose()
        // {
        //     base.OnClose();
        // }

        // public override void OnUpdate()
        // {
        //     base.OnUpdate();

        // }

        public void Close()
        {
            // _animation.Play("UIGuideTextClose");
            HideText( ()=>{
            GameObject.Destroy(this.gameObject);});
			_mask.SetActiveEx(false);
			DictionaryTools.SetContentText(_label, "");
        }
    }
}