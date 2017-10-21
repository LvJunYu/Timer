using System;
using SoyEngine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 65535, Type = typeof(ShadowUnit))]
    public class ShadowUnit : UnitBase
    {
        protected static ShadowUnit _instance;

        public static ShadowUnit Instance
        {
            get { return _instance; }
        }

        protected string _lastAnimName;
        protected Color _color = Color.white;
        protected SkeletonAnimation _skeletonAnimation;
        protected EDirectionType DirectionType;
        protected int _deadFrameIdx;
        protected int _normalDeadFrameIdx;

        public AnimationState AnimationState
        {
            get { return _skeletonAnimation.state; }
        }

        public Color Color
        {
            set
            {
                _color = value;
                _view.SetRendererColor(_color);
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
//            _view.SetRendererColor(_color);
            return true;
        }

        internal override void Reset()
        {
            base.Reset();
            _skeletonAnimation.Reset();
            _deadFrameIdx = -1;
            Speed = IntVec2.zero;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _deadFrameIdx = -1;
            Speed = IntVec2.zero;
        }

        public void UpdatePos(IntVec2 pos)
        {
            if (_curPos != pos)
            {
                _curPos = pos;
                if (_normalDeadFrameIdx > 0)
                {
                    _normalDeadFrameIdx = -1;
                }
            }
        }

        public void UpdateAnim(string animName, bool loop, float timeScale, int trackIdx, int frame)
        {
            if (_skeletonAnimation == null) return;
            if (string.IsNullOrEmpty(animName))
            {
                _skeletonAnimation.state.ClearTrack(trackIdx);
            }
            else
            {
                _skeletonAnimation.state.TimeScale = timeScale;
                _skeletonAnimation.state.SetAnimation(trackIdx, animName, loop);
                _lastAnimName = animName;
                if (animName == "Death")
                {
                    _deadFrameIdx = frame;
                }
            }
        }

        // 编辑模式下试玩残影更新动画组件
        public void EditPlayRecordUpdateAnim(float deltaTime)
        {
            if (_skeletonAnimation != null)
            {
                _skeletonAnimation.Update(deltaTime);
            }
        }

        public void ShadowFinish()
        {
            if (_lastAnimName != "Death" && _skeletonAnimation != null)
            {
                _skeletonAnimation.state.ClearTracks();
            }
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_instance == this) _instance = null;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (GM2DGame.Instance.GameMode.ShadowDataPlayed != null)
            {
                GM2DGame.Instance.GameMode.ShadowDataPlayed.Play(GameRun.Instance.LogicFrameCnt);
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_isAlive)
            {
                UpdateShadowView();
            }
        }

        public void UpdateShadowView()
        {
            if (_trans != null)
            {
                UpdateTransPos();
                if (_deadFrameIdx > 0)
                {
                    if (GameRun.Instance.LogicFrameCnt - _deadFrameIdx == 20)
                    {
                        SpeedY = 150;
                    }
                    if (GameRun.Instance.LogicFrameCnt - _deadFrameIdx > 20)
                    {
                        SpeedY -= 15;
                        if (SpeedY < -160)
                        {
                            SpeedY = -160;
                        }
                        _curPos += Speed;
                        UpdateRot((GameRun.Instance.LogicFrameCnt - _deadFrameIdx - 20) * 0.3f);
                    }
                }
                if (_view != null)
                {
                    if (_normalDeadFrameIdx > 0)
                    {
                        float a = _color.a * (1f - (GameRun.Instance.LogicFrameCnt - _normalDeadFrameIdx) * 0.02f);
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

        public void SetFacingDir(EDirectionType EDirectionType)
        {
            if (DirectionType == EDirectionType)
            {
                return;
            }
            if (_trans == null) return;
            DirectionType = EDirectionType;
            Vector3 euler = Trans.eulerAngles;
            _trans.eulerAngles = DirectionType == EDirectionType.Right
                ? new Vector3(euler.x, 0, euler.z)
                : new Vector3(euler.x, 180, euler.z);
        }

        public void NornalDeath(int frame)
        {
            _normalDeadFrameIdx = frame;
        }

        public SkeletonAnimation CreateSnapShot()
        {
            GameObject snap = Object.Instantiate(_trans.gameObject);
            _view.SetRendererColor(_color);
            if (snap != null)
            {
                SkeletonAnimation anim = snap.GetComponent<SkeletonAnimation>();
                if (anim != null)
                {
                    _skeletonAnimation = anim;
                    return anim;
                }
            }
            return null;
        }

        protected void UpdateRot(float rad)
        {
            float y = DirectionType == EDirectionType.Right
                ? 0
                : 180;
            _trans.rotation = Quaternion.Euler(0, y, rad * Mathf.Rad2Deg);
            IntVec2 size = GetDataSize();
            var up = new Vector2(0, 0.5f * size.y / ConstDefineGM2D.ServerTileScale);
            Vector2 newTransPos = (Vector2) _trans.position + up - (Vector2) _trans.up.normalized * up.y;
            _trans.position = newTransPos;
        }
    }
}