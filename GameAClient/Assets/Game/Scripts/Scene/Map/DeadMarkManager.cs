/********************************************************************
  ** Filename : DeadMarkManager.cs
  ** Author : quan
  ** Date : 11/25/2016 4:46 PM
  ** Summary : DeadMarkManager.cs
  ***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class DeadMarkManager
    {
        public static DeadMarkManager _instance;

        private readonly List<Vector2> _curDeadPosList = new List<Vector2>();
        private readonly List<DeadMark> _deadMarkActiveList = new List<DeadMark>();
        private readonly LinkedList<Vector2> _deadPosList = new LinkedList<Vector2>();
        private readonly Stack<DeadMark> _pool = new Stack<DeadMark>(50);
        private bool _isPlay;
        private LinkedList<Vector2>.Enumerator _iterator;
        private int _leftFrameCount;
        private Transform _tran;

        public static DeadMarkManager Instance
        {
            get { return _instance ?? (_instance = new DeadMarkManager()); }
        }

        public void Dispose()
        {
            foreach (var mark in _deadMarkActiveList)
            {
                if (mark != null)
                {
                    UnityEngine.Object.Destroy(mark.gameObject);
                }
            }
            _deadMarkActiveList.Clear();
            foreach (var mark in _pool)
            {
                if (mark != null)
                {
                    UnityEngine.Object.Destroy(mark.gameObject);
                }
            }
            _pool.Clear();
            if (_tran != null)
            {
                UnityEngine.Object.Destroy(_tran.gameObject);
            }
            if (_instance != null)
            {
                _instance = null;
                Messenger.RemoveListener(EMessengerType.GameFailedDeadMark, OnGameFinishDeadMark);
                Messenger.RemoveListener(EMessengerType.OnMainPlayerDead, OnMainPlayerDead);
                Messenger.RemoveListener(EMessengerType.OnGameRestart, OnGameRestart);
                Messenger.RemoveListener(EMessengerType.OnEdit, OnGameRestart);
            }
        }

        public void Init()
        {
            _isPlay = false;
            _tran = new GameObject("DeadMarkRoot").transform;
            Messenger.AddListener(EMessengerType.GameFailedDeadMark, OnGameFinishDeadMark);
            Messenger.AddListener(EMessengerType.OnMainPlayerDead, OnMainPlayerDead);
            Messenger.AddListener(EMessengerType.OnGameRestart, OnGameRestart);
            Messenger.AddListener(EMessengerType.OnEdit, OnGameRestart);
        }

        public byte[] GetDeadPosition()
        {
            if (_curDeadPosList.Count == 0)
            {
                return new byte[0];
            }
            using (PooledFixedByteBufHolder holder = PoolFactory<PooledFixedByteBufHolder>.Get())
            {
                holder.Content.Clear();
                for (int i = 0; i < _curDeadPosList.Count; i++)
                {
                    Vector2 pos = _curDeadPosList[i];
                    holder.Content.WriteUShort((ushort) (pos.x*10));
                    holder.Content.WriteUShort((ushort) (pos.y*10));
                }
                return holder.Content.ReadableBytesToArray();
            }
        }

        public void OnSync(byte[] deadPosAry)
        {
            if (deadPosAry == null)
            {
                return;
            }
            _deadPosList.Clear();
            using (PooledEmptyByteBufHolder holder = PoolFactory<PooledEmptyByteBufHolder>.Get())
            {
                holder.Content.SetBufForRead(deadPosAry);
                while (holder.Content.CanRead)
                {
                    try
                    {
                        var v = new Vector2();
                        v.x = holder.Content.ReadUShort()*0.1f;
                        v.y = holder.Content.ReadUShort()*0.1f;
                        if (v.x < 1 || v.y < 1)
                        {
                            break;
                        }
                        _deadPosList.AddLast(v);
                    }
                    catch
                    {
                        LogHelper.Error("read dead Pos exception");
                    }
                }
            }
        }

        public void Update()
        {
            if (!_isPlay)
            {
                return;
            }
            if (_leftFrameCount > 0)
            {
                _leftFrameCount--;
                return;
            }
            _leftFrameCount = 1;
            if (_iterator.MoveNext())
            {
                DeadMark dm = GetItem();
                dm.Set(_iterator.Current, _deadMarkActiveList.Count);
                _deadMarkActiveList.Add(dm);
            }
            else
            {
                _isPlay = false;
            }
        }

        public void Play()
        {
            _isPlay = true;
            _iterator = _deadPosList.GetEnumerator();
        }

        public void Stop()
        {
            for (int i = 0; i < _deadMarkActiveList.Count; i++)
            {
                FreeItem(_deadMarkActiveList[i]);
            }
            _deadMarkActiveList.Clear();
            _isPlay = false;
        }

        private void FreeItem(DeadMark item)
        {
            item.gameObject.SetActive(false);
            _pool.Push(item);
        }

        private DeadMark GetItem()
        {
            DeadMark item = null;
            if (_pool.Count > 0)
            {
                item = _pool.Pop();
                item.gameObject.SetActive(true);
            }
            else
            {
                item = DeadMark.CreateInstance(_tran);
            }
            return item;
        }


        private void OnGameFinishDeadMark()
        {
            OnSync(GM2DGame.Instance.Project.DeadPos);
            Play();
        }

        private void OnMainPlayerDead()
        {
            Vector3 deadPos = PlayMode.Instance.MainPlayer.Trans.position + Vector3.up*0.5f;
            _curDeadPosList.Add(new Vector2(deadPos.x, deadPos.y));
        }

        private void OnGameRestart()
        {
            Stop();
            for (int i = 0; i < _curDeadPosList.Count; i++)
            {
                _deadPosList.AddFirst(_curDeadPosList[i]);
                if (_deadPosList.Count > 50)
                {
                    _deadPosList.RemoveLast();
                }
            }
            _curDeadPosList.Clear();
        }
    }
}