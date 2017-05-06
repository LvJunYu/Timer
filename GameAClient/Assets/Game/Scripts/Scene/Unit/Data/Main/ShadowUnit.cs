using System;
using SoyEngine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace GameA.Game
{
    [Serializable]
    [Unit (Id = 65535, Type = typeof (ShadowUnit))]
    public class ShadowUnit : UnitBase
    {
        protected static ShadowUnit _instance;
        protected string _lastAnimName;

        protected Color _color = Color.white;

        protected SkeletonAnimation _animation;
        protected ERotationType RotationType;

        protected int _deadFrameIdx;
        protected int _normalDeadFrameIdx;

        public AnimationState AnimationState {
            get { return _animation.state; }
        }

        public static ShadowUnit Instance {
            get { return _instance; }
        }


        public Color Color {
            set {
                _color = value;
                _view.SetRendererColor (_color);
            }
        }

        protected override bool OnInit ()
        {
            if (!base.OnInit ()) {
                return false;
            }
            //if (_instance != null) return false;
            //_instance = this;
            //_animation = _trans.GetComponent<SkeletonAnimation> ();
            //_view.SetRendererColor (_color);
            return true;
        }


        internal override void Reset ()
        {
            base.Reset ();
            _animation.Reset ();
            _deadFrameIdx = -1;
            Speed = IntVec2.zero;
        }

        internal override void OnPlay ()
        {
            base.OnPlay ();
            _deadFrameIdx = -1;
            Speed = IntVec2.zero;
        }

        public void UpdatePos (IntVec2 pos)
        {
            if (_curPos != pos) {
                _curPos = pos;
                if (_normalDeadFrameIdx > 0) {
                    _normalDeadFrameIdx = -1;
                }
            }           
        }

        public void UpdateAnim (string animName, bool loop, float timeScale, int trackIdx, int frame)
        {
            if (_animation == null) return;
            if (string.IsNullOrEmpty (animName)) {
                _animation.state.ClearTrack (trackIdx);
            } else {
                _animation.state.TimeScale = timeScale;
                _animation.state.SetAnimation (trackIdx, animName, loop);
                _lastAnimName = animName;
                if (animName == "Death") {
                    _deadFrameIdx = frame;
                }
            }
        }

        // 编辑模式下试玩残影更新动画组件
        public void EditPlayRecordUpdateAnim (float deltaTime) {
            if (_animation != null) {
                _animation.Update(deltaTime);
            }
        }

        public void ShadowFinish ()
        {
            if (_lastAnimName != "Death") {
                _animation.state.ClearTracks ();
            }
        }

        //internal override void OnObjectDestroy ()
        //{
        //    base.OnObjectDestroy ();
        //    if (_instance == this) _instance = null;
        //}

        public void UpdateDataView ()
        {
            if (_trans != null) {
                UpdateTransPos();
                if (_deadFrameIdx > 0) {
                    if (PlayMode.Instance.LogicFrameCnt - _deadFrameIdx == 20) {
                        SpeedY = 150;
                    }
                    if (PlayMode.Instance.LogicFrameCnt - _deadFrameIdx > 20) {
                        SpeedY -= 15;
                        if (SpeedY < -160) {
                            SpeedY = -160;
                        }
                        _curPos += Speed;
                        UpdateRotation ((PlayMode.Instance.LogicFrameCnt - _deadFrameIdx - 20) * 0.3f);
                    }
                }
                if (_view != null)
                {
                    if (_normalDeadFrameIdx > 0)
                    {
                        float a = _color.a * (1f - (PlayMode.Instance.LogicFrameCnt - _normalDeadFrameIdx) * 0.02f);
                        if (a < 0) a = 0;
                        _view.SetRendererColor(new Color(_color.r, _color.g, _color.b, a));
                    }
                    if (_normalDeadFrameIdx == -1 && _view != null)
                    {
                        _view.SetRendererColor(_color);
                    }
                }
            }
        }

        //public override void SetFacingDir(ERotationType eRotationType)
        //{
        //    if (RotationType == eRotationType)
        //    {
        //        return;
        //    }
        //    if (_trans == null) return;
        //    RotationType = eRotationType;
        //    Vector3 euler = Trans.eulerAngles;
        //    _trans.eulerAngles = RotationType == ERotationType.Right
        //        ? new Vector3(euler.x, 0, euler.z)
        //        : new Vector3(euler.x, 180, euler.z);
        //}

        public void NornalDeath (int frame)
        {
            _normalDeadFrameIdx = frame;
        }

        public SkeletonAnimation CreateSnapShot () {
            GameObject snap = GameObject.Instantiate(_trans.gameObject) as GameObject;
            if (snap != null) {
                SkeletonAnimation anim = snap.GetComponent<SkeletonAnimation> ();
                if (anim != null) {
                    return anim;
                }
            }
            return null;
        }

        protected void UpdateRotation (float rad)
        {
            float y = RotationType == ERotationType.Right
                ? 0
                : 180;
            _trans.rotation = Quaternion.Euler(0, y, rad * Mathf.Rad2Deg);
            IntVec2 size = GetDataSize();
            var up = new Vector2(0, 0.5f * size.y / ConstDefineGM2D.ServerTileScale);
            Vector2 newTransPos = (Vector2)_trans.position + up - (Vector2)_trans.up.normalized * up.y;
            _trans.position = newTransPos;
        }
    }
}