using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 9001, Type = typeof(Text))]
    public class Text : EffectBase
    {
        private int _timer;
        private TextMesh _textMesh;
        
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
            var go = Object.Instantiate (ResourcesManager.Instance.GetPrefab(EResType.ModelPrefab, 
                ConstDefineGM2D.TextBillboardPrefabName)) as GameObject;
            if (go != null)
            {
                CommonTools.SetParent(go.transform, _trans);
                _textMesh = go.GetComponent<TextMesh>();
                _textMesh.text = DataScene2D.Instance.GetUnitExtra(_guid).Msg;
                _textMesh.GetComponent<Renderer>().sortingOrder = (int) ESortingOrder.EffectItem;
            }
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            ShowHide(false);
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_textMesh != null)
            {
                Object.Destroy(_textMesh.gameObject);
            }
        }

        public override void UpdateExtraData()
        {
            base.UpdateExtraData();
            if (_textMesh != null)
            {
                _textMesh.text = DataScene2D.Instance.GetUnitExtra(_guid).Msg;
            }
        }

        internal override void OnCtrlBySwitch()
        {
            base.OnCtrlBySwitch();
            if (_ctrlBySwitch)
            {
                _timer = 10 * ConstDefineGM2D.FixedFrameCount;
                ShowHide(true);
            }
        }

        public override void UpdateLogic()
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

        private void ShowHide(bool show)
        {
            if (_textMesh != null)
            {
                _textMesh.SetActiveEx(show);
            }
        }
    }
}