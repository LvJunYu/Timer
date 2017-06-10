/********************************************************************
** Filename : Key
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:47:08
** Summary : Key
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;

namespace GameA.Game
{
    [Unit(Id = 5012, Type = typeof(Key))]
    public class Key : CollectionBase
    {
        protected Tweener _tweener;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _tweener = _trans.DOMoveY(_trans.position.y + 0.1f, 0.6f);
            _tweener.Play();
            _tweener.SetLoops(-1, LoopType.Yoyo);
            return true;
        }

        internal override void OnObjectDestroy()
        {
            if (_tweener != null)
            {
                DOTween.Kill(_trans);
                _tweener = null;
            }
            base.OnObjectDestroy();
        }

        protected override void OnTrigger()
        {
            PlayMode.Instance.SceneState.AddKey();
            base.OnTrigger();
        }
    }
}
