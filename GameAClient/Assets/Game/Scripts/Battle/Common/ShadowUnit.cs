﻿using System;
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
        protected Color _color = Color.white;
        protected SkeletonAnimation _skeletonAnimation;
        protected int _deadFrameIdx;
        protected bool _playFinish;

        public static ShadowUnit Instance
        {
            get { return _instance; }
        }

        public override bool IsShadow
        {
            get { return true; }
        }

        public SkeletonAnimation SkeletonAnimation
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

        internal override void Reset()
        {
            base.Reset();
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
            _animation.PlayOnce(Victory);
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_instance == this) _instance = null;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (!_playFinish)
            {
                _shadowData.Play(GameRun.Instance.LogicFrameCnt);
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_view == null || _playFinish) return;
            //死亡后效果
            if (_deadFrameIdx > 0)
            {
                //渐隐消失
                float a = _color.a * (1f - (GameRun.Instance.LogicFrameCnt - _deadFrameIdx) * 0.01f);
                if (a < 0) a = 0;
                _view.SetRendererColor(new Color(_color.r, _color.g, _color.b, a));
            }
            UpdateTransPos();
        }

        public void Revive()
        {
            _deadFrameIdx = 0;
            _view.SetRendererColor(_color);
        }

        public void Dead(int frame)
        {
            _deadFrameIdx = frame;
        }

        private void ClearData()
        {
            _deadFrameIdx = 0;
            Speed = IntVec2.zero;
            _shadowData.PlayClear();
            _playFinish = false;
        }

        public void SetShadowData(ShadowData shadowData)
        {
            _shadowData = shadowData;
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

    }
}