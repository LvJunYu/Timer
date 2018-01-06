using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class WholeRope
    {
        private List<RopeJoint> _joints = new List<RopeJoint>(Rope.JointCount);
        private int _addForceTimer;
        private int _carryPlayerCount;

        public bool IsInterest
        {
            get
            {
                for (int i = 0; i < _joints.Count; i++)
                {
                    if (_joints[i] == null || !_joints[i].IsInterest)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public int Length
        {
            get { return _joints.Count; }
        }

        public UnitBase TieUnit
        {
            get { return _joints[0].PreJoint; }
        }

        public void AddRope(bool create = false)
        {
            if (!create)
            {
                _joints.Capacity += Rope.JointCount;
            }

            for (int j = 0; j < Rope.JointCount; j++)
            {
                _joints.Add(null);
            }
        }

        public void SetJoint(int segmentIndex, RopeJoint[] ropeJoints)
        {
            for (int i = 0; i < ropeJoints.Length; i++)
            {
                int index = segmentIndex * Rope.JointCount + i;
                if (index < _joints.Count)
                {
                    _joints[index] = ropeJoints[i];
                }
                else
                {
                    LogHelper.Error("index is out of range");
                }
            }
        }

        public void RemoveRopeJoint(int segmentIndex)
        {
            for (int i = _joints.Count - 1; i >= segmentIndex * Rope.JointCount; i--)
            {
                _joints.RemoveAt(i);
            }
        }

        public void OnPlay()
        {
            for (int i = 0; i < _joints.Count; i++)
            {
                if (_joints[i] == null)
                {
                    LogHelper.Error("joint == null");
                    return;
                }

                _joints[i].Set(this);
                if (i > 0)
                {
                    _joints[i].SetPreJoint(_joints[i - 1]);
                    _joints[i - 1].SetnNextJoint(_joints[i]);
                }
            }
        }

        public RopeJoint GetJoint(int jointIndex)
        {
            if (jointIndex < _joints.Count)
            {
                return _joints[jointIndex];
            }

            LogHelper.Error("index is out of range");
            return null;
        }

        public void UpdateLogic()
        {
            if (!IsInterest)
            {
                return;
            }
            for (int i = 0; i < _joints.Count; i++)
            {
                _joints[i].UpdateLogic();
            }
        }

        public void UpdateView(float deltaTime, bool addForce = false)
        {
            if (!IsInterest)
            {
                return;
            }

            for (int i = _joints.Count - 1; i >= 0; i--)
            {
                _joints[i].CheckPreJointPos();
            }

            for (int i = 0; i < _joints.Count; i++)
            {
                _joints[i].UpdateView(deltaTime);
            }
        }

        public void AddForce(IntVec2 force, int jointIndex)
        {
            if (jointIndex < _joints.Count)
            {
                _addForceTimer = GameRun.Instance.LogicFrameCnt;
                _joints[jointIndex].Speed += force;
            }
        }

        public bool JustAddForce(int ropeIndex)
        {
            return GameRun.Instance.LogicFrameCnt - _addForceTimer < 10;
        }

        public void OnPlayerClimbRope(bool value)
        {
            if (value)
            {
                _carryPlayerCount++;
            }
            else
            {
                if (_carryPlayerCount > 0)
                {
                    _carryPlayerCount--;
                }
            }
        }
        
        public void Clear()
        {
            _joints.Clear();
        }

    }

    public class RopeManager : IDisposable
    {
        private static RopeManager _instance;
        private int _curDicIndex;
        private Dictionary<int, WholeRope> _ropes = new Dictionary<int, WholeRope>();

        public static RopeManager Instance
        {
            get { return _instance ?? (_instance = new RopeManager()); }
        }

        public RopeManager()
        {
            Messenger<int, bool>.AddListener(EMessengerType.OnPlayerClimbRope, OnPlayerClimbRope);
        }

        public void Reset()
        {
            _curDicIndex = 0;
            foreach (var wholeRope in _ropes.Values)
            {
                wholeRope.Clear();
            }
            _ropes.Clear();
        }

        public void Dispose()
        {
            Messenger<int, bool>.RemoveListener(EMessengerType.OnPlayerClimbRope, OnPlayerClimbRope);
            Reset();
            _instance = null;
        }

        public void RegistRope(ref int ropeIndex, bool createNew = false)
        {
            if (createNew)
            {
                _curDicIndex++;
                ropeIndex = _curDicIndex;
                _ropes.Add(_curDicIndex, new WholeRope());
            }

            WholeRope wholeRope = GetWholeRope(ropeIndex);
            if (wholeRope != null)
            {
                wholeRope.AddRope(createNew);
            }
        }

        public void SetRopeJoint(int ropeIndex, int segmentIndex, RopeJoint[] ropeJoints)
        {
            WholeRope wholeRope = GetWholeRope(ropeIndex);
            if (wholeRope != null)
            {
                wholeRope.SetJoint(segmentIndex, ropeJoints);
            }
        }

        public void RemoveRopeJoint(int ropeIndex, int segmentIndex)
        {
            WholeRope wholeRope = GetWholeRope(ropeIndex);
            if (wholeRope != null)
            {
                wholeRope.RemoveRopeJoint(segmentIndex);
                if (wholeRope.Length == 0)
                {
                    _ropes.Remove(ropeIndex);
                }
            }
        }

        public UnitBase GetUpFloorUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            IntVec2 dataSize = tableUnit.GetDataSize(ref unitDesc);
            IntVec2 centerUpFloorPos = new IntVec2(unitDesc.Guid.x + dataSize.x / 2, unitDesc.Guid.y + dataSize.y + 1);
            Grid2D checkGrid = new Grid2D(centerUpFloorPos, centerUpFloorPos);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].CanRope)
                {
                    return units[i];
                }
            }

            return null;
        }

        public Rope GetDownFloorRope(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            IntVec2 dataSize = tableUnit.GetDataSize(ref unitDesc);
            IntVec2 centerDownFloorPos = new IntVec2(unitDesc.Guid.x + dataSize.x / 2, unitDesc.Guid.y - 1);
            Grid2D checkGrid = new Grid2D(centerDownFloorPos, centerDownFloorPos);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Id == UnitDefine.RollerId)
                {
                    return units[i] as Rope;
                }
            }

            return null;
        }

        public bool CheckTieRope(UnitDesc unitDesc, out Rope rope)
        {
            var tableUnit = TableManager.Instance.GetUnit(unitDesc.Id);
            IntVec2 dataSize = tableUnit.GetDataSize(ref unitDesc);
            IntVec2 centerDownFloorPos = new IntVec2(unitDesc.Guid.x + dataSize.x / 2, unitDesc.Guid.y - 1);
            Grid2D checkGrid = new Grid2D(centerDownFloorPos, centerDownFloorPos);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Id == UnitDefine.RopeId)
                {
                    rope = units[i] as Rope;
                    return true;
                }
            }

            rope = null;
            return false;
        }

        public void OnPlay()
        {
            foreach (var ropeUnit in _ropes.Values)
            {
                ropeUnit.OnPlay();
            }
        }

        public void UpdateLogic()
        {
            foreach (var ropeUnit in _ropes.Values)
            {
                ropeUnit.UpdateLogic();
            }
        }

        public void UpdateView(float deltaTime)
        {
            foreach (var ropeUnit in _ropes.Values)
            {
                ropeUnit.UpdateView(deltaTime);
            }
        }

        private WholeRope GetWholeRope(int ropeIndex)
        {
            WholeRope wholeRope;
            if (_ropes.TryGetValue(ropeIndex, out wholeRope))
            {
                return wholeRope;
            }

            LogHelper.Error("can not find rope of index {0}", ropeIndex);
            return null;
        }

        private void OnPlayerClimbRope(int ropeIndex, bool value)
        {
            WholeRope wholeRope = GetWholeRope(ropeIndex);
            if (wholeRope != null)
            {
                wholeRope.OnPlayerClimbRope(value);
            }
        }
    }
}