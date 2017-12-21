using System;
using SoyEngine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 65535, Type = typeof(ShadowUnit))]
    public class ShadowUnit : UnitBase
    {
        protected static ShadowUnit _instance;
        private static string Victory = "Victory";
        protected ShadowData _shadowData;
        protected Color _color = new Color(0.57f, 0.57f, 0.57f, 1);
        protected SkeletonAnimation _skeletonAnimation;
        protected int _deadFrame;
        protected int _onPortalFrame;
        protected bool _playFinish;
        protected int _aheadFrame;

        protected int _logicFrame
        {
            get { return GameRun.Instance.LogicFrameCnt + _aheadFrame; }
        }

        public static ShadowUnit Instance
        {
            get { return _instance; }
        }

        public override bool IsShadow
        {
            get { return true; }
        }

        public override SkeletonAnimation SkeletonAnimation
        {
            get
            {
                if (_view == null) return null;
                if (_skeletonAnimation == null)
                {
                    _skeletonAnimation = _trans.GetComponent<SkeletonAnimation>();
                }
                return _skeletonAnimation;
            }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            if (_instance != null) return false;
            _instance = this;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _view.SetRendererColor(_color);
            CreatePlayerName();
            if (_playerNameInGame != null)
            {
                _playerNameInGame.SetNameActive(true);
            }
            return true;
        }

        /// <summary>
        /// aHeadFrame影子提前出场的帧数
        /// </summary>
        /// <param name="shadowData"></param>
        /// <param name="aHeadFrame"></param>
        public void SetShadowData(ShadowData shadowData, int aHeadFrame = 0)
        {
            _shadowData = shadowData;
            _aheadFrame = aHeadFrame;
        }

        internal override void Reset()
        {
            base.Reset();
            if (_view != null)
            {
                _view.SetRendererColor(_color);
                if (_playerNameInGame != null)
                {
                    _playerNameInGame.SetNameActive(true);
                }
            }
            SkeletonAnimation.Reset();
            ClearData();
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            ClearData();
        }

        public void UpdatePos(IntVec2 pos)
        {
            _curPos = pos;
        }

        public void UpdateAnim(string animName, bool loop, float timeScale, int trackIdx)
        {
            if (_view == null) return;
            if (string.IsNullOrEmpty(animName)) return;
            if (loop)
            {
                _animation.PlayLoop(animName, timeScale, trackIdx);
            }
            else
            {
                _animation.PlayOnce(animName, timeScale, trackIdx);
            }
        }

        public void ShadowFinish()
        {
            Speed = IntVec2.zero;
            _playFinish = true;
            _animation.PlayLoop(Victory);
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_instance == this) _instance = null;
        }

        public override void UpdateLogic()
        {
            if (_playFinish) return;
            base.UpdateLogic();
            _shadowData.Play(_logicFrame);
        }

        public override void UpdateView(float deltaTime)
        {
            if (_view == null || _playFinish) return;
            //死亡后效果
            if (_deadFrame > 0)
            {
                Fade(_deadFrame, 0.01f);
            }
            if (_onPortalFrame > 0)
            {
                Fade(_onPortalFrame, 0.04f);
            }
            UpdateTransPos();
        }

        private void Fade(int startFram, float speed)
        {
            float a = _color.a * (1f - (_logicFrame - startFram) * speed);
            if (a < 0)
            {
                a = 0;
            }
            _view.SetRendererColor(new Color(_color.r, _color.g, _color.b, a));
        }

        public void Dead(int frame)
        {
            _deadFrame = frame;
            if (_playerNameInGame != null)
            {
                _playerNameInGame.SetNameActive(false);
            }
        }

        public void Revive()
        {
            if (_view == null) return;
            _deadFrame = 0;
            _animation.Reset();
            _view.SetRendererColor(_color);
            if (_playerNameInGame != null)
            {
                _playerNameInGame.SetNameActive(true);
            }
        }

        public void EnterPortal(int frame)
        {
            _onPortalFrame = frame;
            if (_playerNameInGame != null)
            {
                _playerNameInGame.SetNameActive(false);
            }
        }

        public void OutPortal()
        {
            if (_view == null) return;
            _onPortalFrame = 0;
            _animation.Reset();
            _view.SetRendererColor(_color);
            if (_playerNameInGame != null)
            {
                _playerNameInGame.SetNameActive(true);
            }
        }

        private void ClearData()
        {
            _deadFrame = _onPortalFrame = 0;
            Speed = IntVec2.zero;
            _shadowData.PlayClear();
            _playFinish = false;
        }

        public void ClearTrack(int trackIdx)
        {
            _animation.ClearTrack(trackIdx);
        }

        // 编辑模式下试玩残影更新动画组件
        public void EditPlayRecordUpdateAnim(float deltaTime)
        {
            if (SkeletonAnimation != null)
            {
                SkeletonAnimation.Update(deltaTime);
            }
        }

        public void SetName(string name)
        {
            if (_playerNameInGame != null)
            {
                _playerNameInGame.SetName(name);
            }
        }
    }
}