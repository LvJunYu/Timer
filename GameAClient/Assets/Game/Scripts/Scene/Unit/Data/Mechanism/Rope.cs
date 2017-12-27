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

        private const int RopeJointId = 4018;
        private const int SectionCount = 10;
        private RopeJoint[] _ropeJoints = new RopeJoint[SectionCount];
        private UnitBase _tieUnit;
        private Rope _preRope;
        private Rope _nextRope;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (!CheckNeighbor())
            {
//                LogHelper.Error("can not tie this rope!");
//                return false;
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
            var tableUnit = TableManager.Instance.GetUnit(RopeJointId);
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
            for (int i = 0; i < _ropeJoints.Length; i++)
            {
                _ropeJoints[i] =
                    PlayMode.Instance.CreateRuntimeUnit(RopeJointId, startPos + i * offset, Rotation) as RopeJoint;

                if (i == 0)
                {
                    if (_tieUnit == null)
                    {
                        CheckNeighbor();
                    }
                    if (_tieUnit != null)
                    {
                        _ropeJoints[i].SetPreJoint(_tieUnit);
                    }
                }
                else
                {
                    _ropeJoints[i].SetPreJoint(_ropeJoints[i - 1]);
                    _ropeJoints[i - 1].SetnNextJoint(_ropeJoints[i]);
                }
            }
        }

        private bool CheckNeighbor()
        {
            bool canRope = false;
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
                    canRope = true;
                    _tieUnit = units[i];
                    if (units[i].Id == Id)
                    {
                        Rope neighborRope = units[i] as Rope;
                        if (neighborRope != null)
                        {
                            SetPreRope(neighborRope);
                            neighborRope.SetNextRope(this);
                        }
                    }
                    break;
                }
            }
            return canRope;
        }

        private void SetNextRope(Rope rope)
        {
            _preRope = rope;
        }

        private void SetPreRope(Rope rope)
        {
            _nextRope = rope;
        }

        protected override void Clear()
        {
            base.Clear();
            _tieUnit = _preRope = _nextRope = null;
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
        }
    }
}