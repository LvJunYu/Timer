using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class StatusBar : MonoBehaviour
    {
        private static string _fillAmout = "_FillAmount";

        public GameObject ShowMe, PlayerBar, MonsterBar;
        public Transform PlayerCurrentHPTrans, MonsterCurrentHPTrans;
        public TextMesh Name;
        public SpriteRenderer PlayerBg, MonsterBg;

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
                    var sprite = _playeSpriteRenderers[_owner.TeamId].sprite;
                    if (sprite != null)
                    {
                        var val = Mathf.Lerp(sprite.textureRect.x, sprite.textureRect.xMax, _showPerccentage) /
                                  sprite.texture.width;
                        _playeSpriteRenderers[_owner.TeamId].material.SetFloat(_fillAmout, val);
                    }
                }
                if (_owner.IsMonster)
                {
                    var sprite = _monsterSpriteRenderers[_owner.TeamId].sprite;
                    if (sprite != null)
                    {
                        var val = Mathf.Lerp(sprite.textureRect.x, sprite.textureRect.xMax, _showPerccentage) /
                                  sprite.texture.width;
                        _monsterSpriteRenderers[_owner.TeamId].material.SetFloat(_fillAmout, val);
                    }
                }
            }
        }

        void Awake()
        {
            _trans = transform;
            var srs = _trans.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < srs.Length; i++)
            {
                srs[i].sortingOrder = (int) ESortingOrder.DragingItem + 1;
            }
            PlayerBg.sortingOrder = (int) ESortingOrder.DragingItem;
            MonsterBg.sortingOrder = (int) ESortingOrder.DragingItem;
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

            if (_owner.MaxHp != 0)
            {
                SetHP(EHPModifyCase.Set, _owner.Hp, _owner.MaxHp);
            }
            else
            {
                SetHP(EHPModifyCase.Set, 1, 1);
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
                SetHPActive(true);
                SetHP(EHPModifyCase.Set, 1, 1);
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