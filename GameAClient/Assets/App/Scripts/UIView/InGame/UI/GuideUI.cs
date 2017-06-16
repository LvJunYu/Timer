/********************************************************************
** Filename : UIViewCountDown
** Author : cwc
** Date : 2016/09/20 星期二 下午 10:08:37
** Summary : 游戏开始时的倒计时界面
***********************************************************************/
// 测试subtree提交

using UnityEngine;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public partial class GuideUI : MonoBehaviour
    {
        public Animation _animation;

        public UnityEngine.UI.Text _label;

        public RectTransform _mask;

        public RectTransform _rightBtnPos;
        public RectTransform _jumpBtnPos;
        public RectTransform _attackBtnPos;

        public UnityEngine.UI.Image _screenBlock;

        public RectTransform _reward;

        public delegate void AnimEventCB (string animName);
        public event AnimEventCB _animCB;

        public void OnAnimationCallback (string animName) {
            if (_animCB != null)
            {
                _animCB(animName);
            }
        }
    }
}
