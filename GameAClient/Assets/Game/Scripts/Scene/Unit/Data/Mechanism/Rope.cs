using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4017, Type = typeof(Rope))]
    public class Rope : UnitBase
    {
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
            return base.OnInit();
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
                startPos = CenterLeftPos;
                offset = IntVec2.right * size.y;
            }
            else if (Rotation == (int) EDirectionType.Left)
            {
                offset = IntVec2.left * size.y;
                startPos = CenterRightPos + offset;
            }
            else
            {
                offset = IntVec2.down * size.y;
                startPos = CenterUpPos + offset;
            }
            if (!_tied && !CheckTieUnit())
            {
                LogHelper.Error("rope can not tie!");
            }
            for (int i = 0; i < _ropeJoints.Length; i++)
            {
                _ropeJoints[i] =
                    PlayMode.Instance.CreateRuntimeUnit(UnitDefine.RopeJointId, startPos + i * offset, Rotation) as RopeJoint;
                if (_ropeJoints[i] != null)
                {
                    _ropeJoints[i].Set(this,_segmentIndex * JointCount + i);
                    if (i == 0 && _segmentIndex == 0)
                    {
                        _ropeJoints[i].SetPreJoint(_tieUnit);
                    }
                }
            }
            RopeManager.Instance.SetRopeJoint(RopeIndex, SegmentIndex, _ropeJoints);
        }

        public bool CheckTieUnit()
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
                    if (units[i].Id == Id)
                    {
                        Rope neighborRope = units[i] as Rope;
                        if (neighborRope != null)
                        {
                            //保证前面的绳子先注册
                            if (!neighborRope._tied && !neighborRope.CheckTieUnit())
                            {
                                LogHelper.Error("rope can not tie");
                                return false;
                            }
                            SetPreRope(neighborRope);
                            RopeManager.Instance.SetRopeJoint(ref _ropeIndex);
                            neighborRope.SetNextRope(this);
                        }
                    }
                    else
                    {
                        _segmentIndex = 0;
                        RopeManager.Instance.SetRopeJoint(ref _ropeIndex, true);
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
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
            base.Clear();
        }

        internal override void OnObjectDestroy()
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
            base.OnObjectDestroy();
        }
    }
}