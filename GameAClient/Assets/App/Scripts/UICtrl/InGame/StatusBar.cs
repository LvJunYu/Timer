using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class StatusBar : MonoBehaviour
    {
        private static float s_hittingTime = 0.5f;
        private static float s_healingTime = 0.3f;
        private static string _fillAmout = "_FillAmount";

        public GameObject ShowMe, PlayerBar, MonsterBar;
        public Transform PlayerCurrentHPTrans, MonsterCurrentHPTrans;
        public TextMesh Name;

        private Transform _trans;
        private ActorBase _owner;
        private float _showPerccentage = 1;
        private SpriteRenderer[] _playeSpriteRenderers, _monsterSpriteRenderers;
        private EHPShowState _hpState;
        private float _stateTimer;

        private float ShowPerccentage
        {
            set
            {
                _showPerccentage = value;
                if (_owner == null) return;
                if (_owner.IsPlayer)
                {
                    _playeSpriteRenderers[_owner.TeamId].material.SetFloat(_fillAmout, _showPerccentage);
                }
                if (_owner.IsMonster)
                {
                    _monsterSpriteRenderers[_owner.TeamId].material.SetFloat(_fillAmout, _showPerccentage);
                }
            }
        }

        void Awake()
        {
            _trans = transform;
            var srs = _trans.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < srs.Length; i++)
            {
                srs[i].sortingOrder = (int) ESortingOrder.DragingItem;
            }
            SetHPActive(false);
            _playeSpriteRenderers = PlayerCurrentHPTrans.GetComponentsInChildren<SpriteRenderer>(true);
            _monsterSpriteRenderers = MonsterCurrentHPTrans.GetComponentsInChildren<SpriteRenderer>(true);
        }

        void Update()
        {
            _trans.rotation = Quaternion.identity;
//            switch (_hpState)
//            {
//                case EHPShowState.Normal:
//                    break;
//                case EHPShowState.BeingHit:
//                    _stateTimer -= Time.deltaTime;
//                    if (_stateTimer < 0)
//                    {
//                        _hpState = EHPShowState.Normal;
//                    }
//                    break;
//                case EHPShowState.BeingHeal:
//                    _stateTimer -= Time.deltaTime;
//                    if (_stateTimer < 0)
//                    {
//                        _hpState = EHPShowState.Normal;
//                    }
//                    break;
//            }
        }

        public void SetHPActive(bool value)
        {
            _trans.SetActiveEx(value);
            if (value)
            {
                RefreshBar();
            }
        }

        public void RefreshBar()
        {
            if (_owner == null) return;
            if (_owner.IsPlayer)
            {
                for (int i = 0; i < _playeSpriteRenderers.Length; i++)
                {
                    _playeSpriteRenderers[i].SetActiveEx(i == _owner.TeamId);
                }
                if (((PlayerBase) _owner).RoomUser != null)
                {
                    Name.text = ((PlayerBase) _owner).RoomUser.Name;
                }
            }
            if (_owner.IsMonster)
            {
                for (int i = 0; i < _monsterSpriteRenderers.Length; i++)
                {
                    _monsterSpriteRenderers[i].SetActiveEx(i == _owner.TeamId);
                }
            }
        }

        public void SetHP(EHPModifyCase modifyCase, int current, int max)
        {
            current = Mathf.Clamp(current, 0, max);
            ShowPerccentage = current / (float) max;
//            switch (modifyCase)
//            {
//                case EHPModifyCase.Set:
//                    _hpState = EHPShowState.Normal;
//                    ShowPerccentage = _targetPerccentage = current / (float) max;
//                    break;
//                case EHPModifyCase.Hit:
//                    _stateTimer = s_hittingTime;
//                    if (EHPShowState.BeingHit != _hpState)
//                    {
//                        _hpState = EHPShowState.BeingHit;
//                        _targetPerccentage = current / (float) max;
//                    }
//                    break;
//                case EHPModifyCase.Heal:
//                    _stateTimer = s_healingTime;
//                    if (EHPShowState.BeingHeal != _hpState)
//                    {
//                        _hpState = EHPShowState.BeingHeal;
//                        _targetPerccentage = current / (float) max;
//                    }
//                    break;
//            }
        }

        public void SetOwner(ActorBase owner)
        {
            _owner = owner;
            ShowMe.SetActive(_owner.IsMain);
            PlayerBar.SetActive(_owner.IsPlayer);
            MonsterBar.SetActive(_owner.IsMonster);
        }

        public void Reset()
        {
            if (_owner != null)
            {
                SetHP(EHPModifyCase.Set, 1, 1);
                SetHPActive(true);
            }
        }
    }

    /// <summary>
    /// HP显示状态
    /// </summary>
    public enum EHPShowState
    {
        Normal, // 正常
        BeingHit, // 被攻击中
        BeingHeal, // 被治疗中
    }

    /// <summary>
    /// HP设置原因
    /// </summary>
    public enum EHPModifyCase
    {
        Set, // 指定
        Hit, // 被攻击
        Heal, // 被治疗
    }
}