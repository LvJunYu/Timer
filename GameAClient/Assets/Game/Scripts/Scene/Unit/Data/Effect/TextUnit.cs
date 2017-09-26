using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 9001, Type = typeof(TextUnit))]
    public class TextUnit : UnitBase
    {
        private int _timer;
        private TextMesh _textMesh;
        
        public override bool CanControlledBySwitch
        {
            get { return true; }
        }
        
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderFrontest();
            return true;
        }
        
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            var go = Object.Instantiate (JoyResManager.Instance.GetPrefab(EResType.ModelPrefab, 
                ConstDefineGM2D.TextBillboardPrefabName)) as GameObject;
            if (go != null)
            {
                CommonTools.SetParent(go.transform, _trans);
                _textMesh = go.GetComponent<TextMesh>();
                _textMesh.text = DataScene2D.Instance.GetUnitExtra(_guid).Msg;
                _textMesh.GetComponent<Renderer>().sortingOrder = (int) ESortingOrder.EffectItem;
            }
            ShowHide(false);
            _view.SetRendererEnabled(!GameRun.Instance.IsPlay);
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            if (_view != null)
            {
                _view.SetRendererEnabled(false);
            }
        }

        internal override void OnEdit()
        {
            base.OnEdit();
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            ShowHide(false);
        }

        public override void UpdateExtraData()
        {
            base.UpdateExtraData();
            _eActiveState = EActiveState.Deactive;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_textMesh != null)
            {
                Object.Destroy(_textMesh.gameObject);
            }
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            if (_eActiveState == EActiveState.Active)
            {
                _timer = 10 * ConstDefineGM2D.FixedFrameCount;
                ShowHide(true);
            }
        }

        public override void UpdateLogic()
        {
            if (_eActiveState == EActiveState.Deactive)
            {
                if (_timer > 0)
                {
                    _timer--;
                    if (_timer == 0)
                    {
                        ShowHide(false);
                    }
                }
            }
        }

        private void ShowHide(bool show)
        {
            if (_textMesh != null)
            {
                _textMesh.SetActiveEx(show);
            }
        }
    }
}