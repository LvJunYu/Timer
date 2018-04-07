using DG.Tweening;

namespace GameA.Game
{
    [Unit(Id = 6004, Type = typeof(Meat))]
    public class Meat : CollectionBase
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
            other.OnHpChanged(200);
            base.OnTrigger(other);
        }
    }
}