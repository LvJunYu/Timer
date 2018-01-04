using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4017, Type = typeof(Rope))]
    public class Rope : UnitBase
    {
        private bool _isPlaying;

        public override bool CanRope
        {
            get { return true; }
        }

        public int RopeIndex
        {
            get { return _ropeIndex; }
        }

        public int SegmentIndex
        {
            get { return _segmentIndex; }
        }

        public bool Tied
        {
            get { return _tied; }
        }

        public const int JointCount = 10;
        private bool _tied;
        private RopeJoint[] _ropeJoints = new RopeJoint[JointCount];
        private int _ropeIndex;
        private int _segmentIndex;
        private UnitBase _tieUnit;
        private Rope _preRope;
        private Rope _nextRope;

        protected override bool OnInit()
        {
            RopeManager.Instance.AddRope(this);
            //初始化时计算所在绳子的段数，用于限制绳子长度
            UnitBase tieUnit = RopeManager.Instance.GetUpFloorUnit(_unitDesc, _tableUnit);
            if (tieUnit != null)
            {
                if (tieUnit.Id == UnitDefine.RopeId)
                {
                    _segmentIndex = ((Rope) tieUnit).SegmentIndex + 1;
                }
                else
                {
                    _segmentIndex = 0;
                }
            }
            SegmentIndexChanged(_segmentIndex);

            return base.OnInit();
        }

        private void SegmentIndexChanged(int segmentIndex)
        {
            _segmentIndex = segmentIndex;
            _nextRope = RopeManager.Instance.GetDownFloorRope(_unitDesc, _tableUnit);
            if (_nextRope != null)
            {
                if (_nextRope != null)
                {
                    _nextRope.SegmentIndexChanged(segmentIndex + 1);
                }
            }
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (Rotation == (int) EDirectionType.Right)
            {
                _view.Trans.localEulerAngles = new Vector3(0, 0, 90);
            }
            else if (Rotation == (int) EDirectionType.Left)
            {
                _view.Trans.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                _view.Trans.localEulerAngles = Vector3.zero;
            }

            if (GameRun.Instance.IsPlaying)
            {
                _view.SetRendererEnabled(false);
            }

            return true;
        }

        internal override void OnPlay()
        {
            if (_view != null)
            {
                _view.SetRendererEnabled(false);
            }

            var tableUnit = TableManager.Instance.GetUnit(UnitDefine.RopeJointId);
            var size = tableUnit.GetDataSize(0, Vector2.one);
            IntVec2 offset, startPos;
            if (Rotation == (int) EDirectionType.Right)
            {
                offset = IntVec2.right * size.y;
                startPos = CenterLeftPos + new IntVec2(0, -size.x / 2);
            }
            else if (Rotation == (int) EDirectionType.Left)
            {
                offset = IntVec2.left * size.y;
                startPos = CenterRightPos + new IntVec2(-size.y, -size.x / 2);
            }
            else
            {
                offset = IntVec2.down * size.y;
                startPos = CenterUpPos + new IntVec2(-size.x / 2, -size.y);
            }

            if (!_tied && !CheckRopeTie())
            {
                LogHelper.Error("rope can not tie!");
                PlayMode.Instance.DestroyUnit(this);
                return;
            }

            for (int i = 0; i < _ropeJoints.Length; i++)
            {
                var jointPos = startPos + i * offset;
                _ropeJoints[i] =
                    PlayMode.Instance.CreateRuntimeUnit(UnitDefine.RopeJointId, jointPos, Rotation) as RopeJoint;
                if (_ropeJoints[i] != null)
                {
                    _ropeJoints[i].Set(this, _segmentIndex * JointCount + i, jointPos);
                    if (i == 0 && _segmentIndex == 0)
                    {
                        _ropeJoints[i].SetPreJoint(_tieUnit);
                    }
                }
            }

            RopeManager.Instance.SetRopeJoint(RopeIndex, SegmentIndex, _ropeJoints);
        }

        public bool CheckRopeTie()
        {
            _tied = false;
            Grid2D checkGrid;
            if (Rotation == (int) EDirectionType.Left)
            {
                checkGrid = new Grid2D(CenterRightPos + IntVec2.right, CenterRightPos + IntVec2.right);
            }
            else if (Rotation == (int) EDirectionType.Right)
            {
                checkGrid = new Grid2D(CenterLeftPos + IntVec2.left, CenterLeftPos + IntVec2.left);
            }
            else
            {
                checkGrid = new Grid2D(CenterUpFloorPos, CenterUpFloorPos);
            }

            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer, float.MinValue,
                float.MaxValue, _dynamicCollider);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].CanRope)
                {
                    _tied = true;
                    _tieUnit = units[i];
                    if (units[i].Id == UnitDefine.RopeId)
                    {
                        Rope neighborRope = units[i] as Rope;
                        if (neighborRope != null)
                        {
                            //保证前面的绳子先注册
                            if (!neighborRope._tied && !neighborRope.CheckRopeTie())
                            {
                                LogHelper.Error("rope can not tie");
                                return false;
                            }

                            SetPreRope(neighborRope);
                            RopeManager.Instance.RegistRope(ref _ropeIndex);
                            neighborRope.SetNextRope(this);
                        }
                    }
                    else
                    {
                        _segmentIndex = 0;
                        RopeManager.Instance.RegistRope(ref _ropeIndex, true);
                    }

                    break;
                }
            }

            return Tied;
        }

        private void SetNextRope(Rope rope)
        {
            _nextRope = rope;
        }

        private void SetPreRope(Rope rope)
        {
            _tied = true;
            _tieUnit = _preRope = rope;
            _ropeIndex = _preRope.RopeIndex;
            _segmentIndex = _preRope.SegmentIndex + 1;
        }

        private void TieUnitDestroy()
        {
            _tieUnit = null;
            _preRope = null;
            _tied = false;
            if (_nextRope != null)
            {
                _nextRope.TieUnitDestroy();
            }
        }

        protected override void Clear()
        {
            _tied = false;
            _tieUnit = _preRope = _nextRope = null;
            base.Clear();
        }

        internal override void OnEdit()
        {
            base.OnEdit();
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
        }

        internal override void OnDispose()
        {
            if (_nextRope != null)
            {
                _nextRope.TieUnitDestroy();
            }

            if (_preRope != null)
            {
                _preRope.SetNextRope(null);
            }

            if (_tied)
            {
                RopeManager.Instance.RemoveRopeJoint(_ropeIndex, _segmentIndex);
            }

            RopeManager.Instance.RemoveRope(this);
            base.OnDispose();
        }
    }
}