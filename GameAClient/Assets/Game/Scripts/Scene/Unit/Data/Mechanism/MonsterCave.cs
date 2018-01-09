using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 5022, Type = typeof(MonsterCave))]
    public class MonsterCave : BlockBase
    {
        private const int MaxCreateCount = 1;
        private List<MonsterBase> _addedMonsters = new List<MonsterBase>(MaxCreateCount);
        private int _timer = -1;

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
            _timer = 50;
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
            if (_timer > 0)
            {
                _timer--;
            }
            else
            {
                if (_addedMonsters.Count < MaxCreateCount)
                {
                    CreateMonster(2001);
                }
            }

            base.UpdateLogic();
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void CreateMonster(int monsterId)
        {
            var monster = PlayMode.Instance.CreateRuntimeUnit(monsterId, _curPos) as MonsterBase;
            _addedMonsters.Add(monster);
            _timer = 200;
            monster.OnPlay();
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = -1;
            _addedMonsters.Clear();
        }
    }
}