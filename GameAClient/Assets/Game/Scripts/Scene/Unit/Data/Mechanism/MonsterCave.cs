using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5022, Type = typeof(MonsterCave))]
    public class MonsterCave : Magic
    {
        private int _maxCreateCount;
        private int _maxAliveCount;
        private int _monsterId;
        private int _intervalTime;
        private int _createdNum;
        private List<MonsterBase> _addedMonsters = new List<MonsterBase>();
        private int _timer = -1;

        public override bool UseMagic()
        {
            return true;
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            SetSortingOrderBackground();
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _timer = 0;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (_withEffect != null)
            {
                SetRelativeEffectPos(_withEffect.Trans, (EDirectionType) Rotation);
            }

            return true;
        }

        public override void UpdateLogic()
        {
            if (_eActiveState != EActiveState.Active)
            {
                return;
            }
            if (_timer > 0)
            {
                _timer--;
            }
            else
            {
                if (_addedMonsters.Count < _maxAliveCount && _createdNum < _maxCreateCount && CheckDoorSpace())
                {
                    CreateMonster(_monsterId);
                }
            }

            base.UpdateLogic();
        }

        private void CreateMonster(int monsterId)
        {
            var monster = PlayMode.Instance.CreateRuntimeUnit(monsterId, _curPos) as MonsterBase;
            if (monster != null)
            {
                monster.SetCave(this);
                _addedMonsters.Add(monster);
                _timer = _intervalTime;
                monster.OnPlay();
                _createdNum++;
            }
        }

        private bool CheckDoorSpace()
        {
            var table = TableManager.Instance.GetUnit(_monsterId);
            var grid = table.GetColliderGrid(_curPos.x, _curPos.y, 0, Vector2.one);
            var colliderNode = NodeFactory.GetColliderNode(_unitDesc, _tableUnit);
            SceneNode sceneNode;
            return !ColliderScene2D.GridCast(grid, out sceneNode, JoyPhysics2D.LayMaskAll, float.MinValue,
                float.MaxValue, colliderNode);
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _addedMonsters.Capacity = _maxAliveCount = unitExtra.MaxAliveMonster;
            _maxCreateCount = unitExtra.MaxCreatedMonster;
            _monsterId = unitExtra.MonsterId;
            _intervalTime = TableConvert.GetTime(unitExtra.MonsterIntervalTime);
            return unitExtra;
        }

        protected override void Clear()
        {
            base.Clear();
            _createdNum = 0;
            _timer = -1;
            _addedMonsters.Clear();
        }

        public void OnMonsterDestroy(MonsterBase monster)
        {
            if (_addedMonsters.Contains(monster))
            {
                _addedMonsters.Remove(monster);
            }
        }

//        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
//        {
//            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
//            return base.OnUpHit(other, ref y, checkOnly);
//        }
//
//        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
//        {
//            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
//            return base.OnDownHit(other, ref y, checkOnly);
//        }
//
//        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
//        {
//            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
//            return base.OnLeftHit(other, ref x, checkOnly);
//        }
//
//        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
//        {
//            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
//            return base.OnRightHit(other, ref x, checkOnly);
//        }
    }
}