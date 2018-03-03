using System;
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
        private List<UnitBase> _itemList = new List<UnitBase>();
        private SpriteRenderer _itemRenderer;
        private SpriteRenderer _lightRenderer;

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

            string spriteName = _itemList.Count == 1 ? _itemList[0].TableUnit.Icon : QuestionSprite;

            Sprite sprite;
            if (JoyResManager.Instance.TryGetSprite(spriteName, out sprite))
            {
                _itemRenderer.sprite = sprite;
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

            string spriteName = _eActiveState == EActiveState.Active ? Ligt1Sprite : Ligt2Sprite;

            Sprite sprite;
            if (JoyResManager.Instance.TryGetSprite(spriteName, out sprite))
            {
                _lightRenderer.sprite = sprite;
            }
        }
    }
}