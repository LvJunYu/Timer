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

        public int Length
        {
            get { return _joints.Count; }
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

        public void UpdateView(float deltaTime, bool addForce = false)
        {
            for (int i = _joints.Count - 1; i >= 0; i--)
            {
                _joints[i].CheckPos();
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
                _joints[jointIndex].Speed += force * 8;
            }

//            for (int i = jointIndex; i < Length; i++)
//            {
//                var delta = 1 - (i - jointIndex) / (float) Length;
//                if (delta > 0)
//                {
//                    _joints[i].Speed += force * delta;
//                }
//            }
//            for (int i = jointIndex - 1; i >= 0; i--)
//            {
//                var delta = 1 - (jointIndex - i) / (float) Length;
//                if (delta > 0)
//                {
//                    _joints[i].Speed += force * delta;
//                }
//            }
        }

        private bool CheckAddForce(int ropeIndex)
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
    }

    public class RopeManager : IDisposable
    {
        private static RopeManager _instance;
        private int _curDicIndex;
        private Dictionary<int, WholeRope> _ropes = new Dictionary<int, WholeRope>();
        private Dictionary<IntVec3, Rope> _ropeUnits = new Dictionary<IntVec3, Rope>();

        public static RopeManager Instance
        {
            get { return _instance ?? (_instance = new RopeManager()); }
        }

        public Dictionary<IntVec3, Rope> RopeUnits
        {
            get { return _ropeUnits; }
        }

        public RopeManager()
        {
            Messenger<int, bool>.AddListener(EMessengerType.OnPlayerClimbRope, OnPlayerClimbRope);
        }

        public void Reset()
        {
            _curDicIndex = 0;
            _ropes.Clear();
        }

        public void Dispose()
        {
            Messenger<int, bool>.RemoveListener(EMessengerType.OnPlayerClimbRope, OnPlayerClimbRope);
            Reset();
            _ropeUnits.Clear();
            _instance = null;
        }

        public void RegisterRope(ref int ropeIndex, bool createNew = false)
        {
            if (createNew)
            {
                _curDicIndex++;
                _ropes.Add(_curDicIndex, new WholeRope());
                ropeIndex = _curDicIndex;
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

        public bool CheckTieUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            IntVec2 dataSize = tableUnit.GetDataSize(ref unitDesc);
            IntVec2 centerUpFloorPos = new IntVec2(unitDesc.Guid.x + dataSize.x / 2, unitDesc.Guid.y + dataSize.y + 1);
            Grid2D checkGrid = new Grid2D(centerUpFloorPos, centerUpFloorPos);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer);
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].CanRope)
                {
                    return true;
                }
            }

            return false;
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

        public void CheckAllRopeTie()
        {
            foreach (var rope in _ropeUnits.Values)
            {
                if (!rope.Tied && !rope.CheckRopeTie())
                {
                    LogHelper.Error("rope can not tie!");
                }
            }
        }

        public void AddForce(IntVec2 force, int ropeIndex, int jointIndex)
        {
            WholeRope wholeRope = GetWholeRope(ropeIndex);
            if (wholeRope != null)
            {
                wholeRope.AddForce(force, jointIndex);
            }
        }

        // todo 判断Interest
        public void UpdateView(float deltaTime)
        {
            foreach (var ropeUnit in _ropes.Values)
            {
                ropeUnit.UpdateView(deltaTime);
            }
        }

        public int GetRopeLength(int ropeIndex)
        {
            WholeRope wholeRope = GetWholeRope(ropeIndex);
            if (wholeRope != null)
            {
                return wholeRope.Length;
            }

            return 0;
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

        public void AddRope(Rope rope)
        {
            if (!_ropeUnits.ContainsKey(rope.Guid))
            {
                _ropeUnits.Add(rope.Guid, rope);
            }
        }

        public void RemoveRope(Rope rope)
        {
            if (_ropeUnits.ContainsKey(rope.Guid))
            {
                _ropeUnits.Remove(rope.Guid);
            }
        }
    }
}