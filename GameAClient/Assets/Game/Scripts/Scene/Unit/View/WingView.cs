using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 2, PreferedPoolSize = 10, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class WingView : IPoolableObject
    {
        protected SpriteRenderer _spriteRenderer;
        protected Transform _trans;

        public Transform Trans
        {
            get { return _trans; }
        }

        public WingView()
        {
            _trans = new GameObject(GetType().Name).transform;
            if (UnitManager.Instance != null) 
            {
                _trans.parent = UnitManager.Instance.GetOriginParent ();
            }
            _spriteRenderer = _trans.gameObject.AddComponent<SpriteRenderer>();
        }
        
        public bool Init(string path)
        {
            Sprite sprite;
            if (!ResourcesManager.Instance.TryGetSprite(path, out sprite))
            {
                LogHelper.Error("TryGetSprite failed,{0}", path);
                return false;
            }
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = (int) ESortingOrder.Item;
            return true;
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sprite = null;
            }
            if (_trans != null)
            {
                _trans.position = UnitDefine.HidePos;
                _trans.localScale = Vector3.one;
                _trans.localRotation = Quaternion.identity;
                _trans.parent = UnitManager.Instance.GetOriginParent();
            }
        }

        public void OnDestroyObject()
        {
        }
    }
}