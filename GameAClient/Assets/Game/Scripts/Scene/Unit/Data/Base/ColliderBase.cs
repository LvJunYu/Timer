using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class ColliderBase
    {
        [SerializeField]
        protected IntVec2 _colliderPos;
        protected Grid2D _colliderGrid;
        protected Grid2D _colliderGridInner;
        protected Grid2D _lastColliderGrid;
    }
}