/********************************************************************
** Filename : Key
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:47:08
** Summary : Key
***********************************************************************/

using DG.Tweening;

namespace GameA.Game
{
    [Unit(Id = 5012, Type = typeof(Key))]
    public class Key : CollectionBase
    {
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

        protected override void OnTrigger(UnitBase other)
        {
            PlayMode.Instance.SceneState.AddKey(other as PlayerBase);
            base.OnTrigger(other);
        }
    }
}