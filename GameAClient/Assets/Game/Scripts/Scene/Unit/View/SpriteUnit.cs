/********************************************************************
** Filename : SpriteUnit
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:38:32
** Summary : SpriteUnit
***********************************************************************/

using DG.Tweening;
using SoyEngine;
using UnityEngine;
using NewResourceSolution;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 30, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class SpriteUnit : UnitView
    {
        protected SpriteRenderer _spriteRenderer;

        public SpriteUnit()
        {
            _spriteRenderer = _trans.gameObject.AddComponent<SpriteRenderer>();
        }

        protected override bool OnInit()
        {
            Sprite sprite = null;
            if (!ResourcesManager.Instance.TryGetSprite(_unit.AssetPath, out sprite))
            {
                LogHelper.Error("TryGetSpriteByName failed,{0}", _unit.AssetPath);
                return false;
            }
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = UnitManager.Instance.GetSortingOrder(_unit.TableUnit);
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
            if (!ResourcesManager.Instance.TryGetSprite(assetPath, out sprite))
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

        public override void SetRendererColor (Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}