/********************************************************************
** Filename : SpriteUnit
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:38:32
** Summary : SpriteUnit
***********************************************************************/

using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 30, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class SpriteUnit : UnitView
    {
        protected SpriteRenderer _spriteRenderer;

        public override void OnGet()
        {
            base.OnGet();
            _spriteRenderer = _trans.gameObject.AddComponent<SpriteRenderer>();
        }

        protected override bool OnInit()
        {
            var tableUnit = _unit.TableUnit;
            string assetPath = tableUnit.Model;
            if (tableUnit.Id == 4001 || tableUnit.Id == 4002)
            {
                assetPath = string.Format("{0}_{1}", tableUnit.Model, Random.Range(1,3));
            }
            Sprite sprite;
            if (!GameResourceManager.Instance.TryGetSpriteByName(assetPath, out sprite))
            {
                LogHelper.Error("TryGetSpriteByName failed,{0}", assetPath);
                return false;
            }
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = UnitManager.Instance.GetSortingOrder(tableUnit);
            return true;
        }

        public override void SetRendererEnabled(bool value)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = value;
            }
        }

        public override void SetSortingOrder(int sortingOrder)
        {
            _spriteRenderer.sortingOrder = sortingOrder;
        }

        public override void OnFree()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sprite = null;
            }
            base.OnFree();
        }

        public override void ChangeView(string assetPath)
        {
            Sprite sprite;
            if (!GameResourceManager.Instance.TryGetSpriteByName(assetPath, out sprite))
            {
                LogHelper.Error("TryGetSpriteByName failed,{0}", assetPath);
                return;
            }
            _spriteRenderer.sprite = sprite;
        }

	    public override void OnSelect()
	    {
		    base.OnSelect();
		    _spriteRenderer.color = SelectedColor;
	    }

	    public override void OnCancelSelect()
	    {
		    base.OnCancelSelect();
			_spriteRenderer.color = NormalColor;
		}
    }
}