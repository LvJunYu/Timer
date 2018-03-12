using System;
using System.Collections;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 5025, Type = typeof(SurpriseBox))]
    public class SurpriseBox : Box
    {
        private const string QuestionSprite = "M1SurpriseBox_Question";
        private const string Ligt1Sprite = "M1SurpriseBox_Light1";
        private const string Ligt2Sprite = "M1SurpriseBox_Light2";
        private const string OpenSprite = "M1SurpriseBox_Open";
        private const string CloseSprite = "M1SurpriseBox";
        private const string OpeneEffectStr = "M1EffectSurpriseBox";
        private const float OpenSecond = 0.5f;
        private static WaitForSeconds OpenTime = new WaitForSeconds(OpenSecond);

        private List<ushort> _itemList = new List<ushort>();
        private SpriteRenderer _itemRenderer;
        private SpriteRenderer _lightRenderer;
        private float _interval;
        private bool _random;
        private bool _limit;
        private int _maxCount;
        private int _curCount;
        private int _timer;
        private UnityNativeParticleItem _openEffect;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            SetItemView();
            SetLightView();
            return true;
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            SetLightView();
        }

        internal override void OnObjectDestroy()
        {
            if (_itemRenderer != null)
            {
                Object.Destroy(_itemRenderer.gameObject);
                _itemRenderer = null;
            }

            if (_lightRenderer != null)
            {
                Object.Destroy(_lightRenderer.gameObject);
                _lightRenderer = null;
            }

            base.OnObjectDestroy();
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _interval = unitExtra.SurpriseBoxInterval;
            _random = unitExtra.IsRandom;
            _limit = unitExtra.SurpriseBoxCountLimit;
            _maxCount = _limit ? unitExtra.SurpriseBoxMaxCount : int.MaxValue;
            _itemList = unitExtra.SurpriseBoxItems.ToList<ushort>();
            return unitExtra;
        }

        internal override void OnPlay()
        {
            RefreshView();
            base.OnPlay();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_curCount < _maxCount)
            {
                if (_timer > 0)
                {
                    _timer--;
                }
                else
                {
                    if (DoSurprise())
                    {
                        _timer = (int) (_interval * ConstDefineGM2D.FixedFrameCount);
                        _curCount++;
                    }
                    else
                    {
                        _timer = 19;
                    }
                }
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            _curCount = 0;
        }

        private bool DoSurprise()
        {
            var count = _itemList.Count;
            if (count == 0)
            {
                return false;
            }

            int index;
            if (_random)
            {
                index = GameATools.GetRandomByValue(GameRun.Instance.LogicFrameCnt, count - 1);
            }
            else
            {
                index = _curCount % count;
            }

            if (index < count)
            {
                var id = _itemList[index];
                if (CheckSpaceValid(id))
                {
                    var item = PlayMode.Instance.CreateRuntimeUnit(id, new IntVec2(_curPos.x, CenterUpFloorPos.y));
                    if (item != null)
                    {
                        ShowOpenEffect();
                        CoroutineProxy.Instance.StartCoroutine(ShowOpenView(item));
                        item.OnPlay();
                        return true;
                    }
                }
            }

            return false;
        }

        private IEnumerator ShowOpenView(UnitBase unit)
        {
            if (_view != null)
            {
                unit.IsAlive = false;
                _view.ChangeView(OpenSprite);
                yield return OpenTime;
                unit.IsAlive = true;
                _view.ChangeView(CloseSprite);
            }
        }

        private void ShowOpenEffect()
        {
            if (_openEffect == null)
            {
                _openEffect = GameParticleManager.Instance.GetUnityNativeParticleItem(OpeneEffectStr, _trans);
                if (_openEffect != null)
                {
                    SetRelativeEffectPos(_openEffect.Trans, EDirectionType.Up);
                    _openEffect.Trans.localPosition += Vector3.up * 0.5f;
                }
            }

            if (_openEffect != null)
            {
                _openEffect.Play(OpenSecond);
            }
        }

        private bool CheckSpaceValid(int id)
        {
            var checkGrid = GM2DTools.CalculateFireColliderGrid(id, _colliderGrid, _unitDesc.Rotation);
            using (var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.ItemLayer))
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].IsAlive)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void RefreshView()
        {
            SetItemView();
            SetLightView();
        }

        private void SetItemView()
        {
            if (_view == null) return;
            if (_itemRenderer == null)
            {
                _itemRenderer = new GameObject("Item").AddComponent<SpriteRenderer>();
                CommonTools.SetParent(_itemRenderer.transform, _trans);
                _itemRenderer.sortingOrder = (int) ESortingOrder.Item;
                _itemRenderer.transform.localPosition = new Vector3(0, 0, -0.01f);
            }

            string spriteName = _itemList.Count == 1
                ? UnitManager.Instance.GetTableUnit(_itemList[0]).Icon
                : QuestionSprite;

            Sprite sprite;
            if (JoyResManager.Instance.TryGetSprite(spriteName, out sprite))
            {
                _itemRenderer.sprite = sprite;
                if (_itemList.Count == 1)
                {
                    _itemRenderer.transform.localScale = Vector3.one * 0.5f;
                    _itemRenderer.transform.localPosition = new Vector3(-0.145f, -0.125f, -0.01f);
                }
                else
                {
                    _itemRenderer.transform.localScale = Vector3.one;
                    _itemRenderer.transform.localPosition = new Vector3(0, 0, -0.01f);
                }
            }
        }

        private void SetLightView()
        {
            if (_view == null) return;
            if (_lightRenderer == null)
            {
                _lightRenderer = new GameObject("Light").AddComponent<SpriteRenderer>();
                CommonTools.SetParent(_lightRenderer.transform, _trans);
                _lightRenderer.sortingOrder = (int) ESortingOrder.Item;
                _lightRenderer.transform.localPosition = new Vector3(0, 0, -0.01f);
            }

            string spriteName;
            if (_eActiveState == EActiveState.Active && _itemList.Count > 0)
            {
                spriteName = Ligt1Sprite;
            }
            else
            {
                spriteName = Ligt2Sprite;
            }

            Sprite sprite;
            if (JoyResManager.Instance.TryGetSprite(spriteName, out sprite))
            {
                _lightRenderer.sprite = sprite;
            }
        }
    }
}