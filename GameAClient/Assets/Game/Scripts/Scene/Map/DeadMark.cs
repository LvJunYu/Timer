  /********************************************************************
  ** Filename : DeadMark.cs
  ** Author : quan
  ** Date : 11/25/2016 4:27 PM
  ** Summary : DeadMark.cs
  ***********************************************************************/

using System.Collections.Generic;

using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using System.Collections;
using SoyEngine;
using System;
using DG.Tweening;

namespace GameA.Game
{
    public class DeadMark : MonoBehaviour
    {
        private Sequence _sequence;
        private Transform _tran;
        private SpriteRenderer _spriteRenderer;

        public DeadMark()
        {
        }

        public void Set(Vector2 pos, int inx)
        {
            TryInit();
            ResetStatus();
            _tran.position = new Vector3(pos.x, pos.y, inx * -0.001f);
            _sequence.Play();
        }

        private void TryInit()
        {
            if(_tran == null)
            {
                _tran = GetComponent<Transform>();
            }
            if(_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            if(_sequence != null)
            {
                return;
            }
            Tweener tweener = null;
            _sequence = DOTween.Sequence();
            tweener = _tran.DOScale(4, 0.3f);
            tweener.SetEase(Ease.OutQuad);
            _sequence.Append(tweener);
            tweener = _tran.DOScale(1, 0.2f);
            tweener.SetEase(Ease.InOutQuad);
            _sequence.Append(tweener);
            _sequence.SetAutoKill(false);
            _sequence.Pause();
        }

        private void ResetStatus()
        {
            _tran.localScale = Vector3.zero;
            _sequence.Rewind();
        }

        private void OnDestroy()
        {
            _sequence.Kill();
            _sequence = null;
        }

        public static DeadMark CreateInstance(Transform parent)
        {
            GameObject go = new GameObject("DeadMark");
            go.transform.SetParent(parent);
            DeadMark dm = go.AddComponent<DeadMark>();
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = (int)ESortingOrder.DeadMark;
            Sprite s = null;
            GameResourceManager.Instance.TryGetSingleSprite("death", out s);
            sr.sprite = s;
            return dm;
        }
    }
}

