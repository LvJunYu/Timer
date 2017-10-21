/********************************************************************
** Filename : ShadowData
** Author : Dong
** Date : 2017/3/4 星期六 下午 7:35:59
** Summary : ShadowData
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    // 影子数据，记录并回放上次游戏表现
    public class ShadowData
    {
        [Serializable]
        public struct AnimRec
        {
            public int FrameIdx;
            public byte NameIdx;
            public byte TimeScale;
            public byte TrackIdx;
            public bool Loop;

            public AnimRec(int frameIdx, byte nameIdx, bool loop, byte timeScale, byte trackIdx)
            {
                FrameIdx = frameIdx;
                NameIdx = nameIdx;
                TimeScale = timeScale;
                TrackIdx = trackIdx;
                Loop = loop;
            }
        }

        [SerializeField] private bool _recording;
        [SerializeField] private List<IntVec2> _posRec = new List<IntVec2>();
        [SerializeField] private List<AnimRec> _animRec = new List<AnimRec>();
        private Dictionary<string, byte> _animName2Idx = new Dictionary<string, byte>();
        private Dictionary<byte, string> _idx2AnimName = new Dictionary<byte, string>();
        private int _curAnimInx;

        public bool IsRecording
        {
            get { return _recording; }
        }

        public ShadowData()
        {
        }

        public ShadowData(RecShadowData recShadowData)
        {
            for (int i = 0; i < recShadowData.PosData.Count; i++)
            {
                _posRec.Add(GM2DTools.ToEngine(recShadowData.PosData[i]));
            }
            for (int i = 0; i < recShadowData.AnimData.Count; i++)
            {
                _animRec.Add(GM2DTools.ToEngine(recShadowData.AnimData[i]));
            }
            for (int i = 0; i < recShadowData.AnimNames.Count; i++)
            {
                byte inx = (byte) i;
                _idx2AnimName.Add(inx, recShadowData.AnimNames[i]);
                _animName2Idx.Add(recShadowData.AnimNames[i], inx);
            }
        }

        public void RecordStart()
        {
            _posRec.Clear();
            _animRec.Clear();
            _animName2Idx.Clear();
            _idx2AnimName.Clear();
            _curAnimInx = 0;
            _recording = true;
        }

        public void RecordDone()
        {
            _recording = false;
        }

        public int Play(int frame)
        {
            if (_recording) return -1;
            if (_posRec.Count <= frame && ShadowUnit.Instance != null)
            {
                ShadowUnit.Instance.ShadowFinish();
                return 0;
            }
            if (ShadowUnit.Instance != null)
            {
                ShadowUnit.Instance.UpdatePos(_posRec[frame]);

                if (_curAnimInx < _animRec.Count)
                {
                    if (frame >= _animRec[_curAnimInx].FrameIdx)
                    {
                        if (_animRec[_curAnimInx].NameIdx == 98)
                        {
                            ShadowUnit.Instance.NornalDeath(frame);
                        }
                        else if (_animRec[_curAnimInx].NameIdx == 99)
                        {
                            ShadowUnit.Instance.UpdateAnim(null, false, 1, _animRec[_curAnimInx].TrackIdx, frame);
                        }
                        else if (_animRec[_curAnimInx].NameIdx == 100)
                        {
                            ShadowUnit.Instance.SetFacingDir(EMoveDirection.Left);
                        }
                        else if (_animRec[_curAnimInx].NameIdx == 101)
                        {
                            ShadowUnit.Instance.SetFacingDir(EMoveDirection.Right);
                        }
                        else
                        {
                            string animName;
                            if (_idx2AnimName.TryGetValue(_animRec[_curAnimInx].NameIdx, out animName))
                            {
                                ShadowUnit.Instance.UpdateAnim(animName, _animRec[_curAnimInx].Loop,
                                    _animRec[_curAnimInx].TimeScale * 0.01f, _animRec[_curAnimInx].TrackIdx, frame);
                            }
                        }
                        _curAnimInx++;
                    }
                }
            }
            return 1;
        }

        public void RecordPos(IntVec2 pos)
        {
            _posRec.Add(pos);
        }

        public void RecordAnimation(string animName, bool loop, float timeScale = 1f, int trackIdx = 0)
        {
            if (timeScale > 1.28f)
            {
                timeScale = 1.28f;
            }
            else if (timeScale < 0)
            {
                timeScale = 0.01f;
            }
            byte idx;
            if (!_animName2Idx.TryGetValue(animName, out idx))
            {
                _animName2Idx.Add(animName, (byte) _animName2Idx.Count);
                _idx2AnimName.Add((byte) _idx2AnimName.Count, animName);
                idx = (byte) (_idx2AnimName.Count - 1);
            }
            RecordAnim(idx, loop, (byte) (timeScale * 100), (byte) trackIdx);
        }

        public void RecordClearAnimTrack(int trackIdx)
        {
            RecordAnim(99, false, 0, (byte) trackIdx);
        }

        public void RecordNormalDeath()
        {
            RecordAnim(98, false, 0, 0);
        }

        public void RecordDirChange(EMoveDirection facingDir)
        {
            RecordAnim((byte) (facingDir == EMoveDirection.Left ? 100 : 101), false, 0, 0);
        }

        private void RecordAnim(byte nameIdx, bool loop, byte timeScale, byte trackIdx)
        {
            _animRec.Add(new AnimRec(GameRun.Instance.LogicFrameCnt, nameIdx, loop, timeScale, trackIdx));
        }

        public RecShadowData GetRecShadowData()
        {
            RecShadowData recShadowData = new RecShadowData();
            for (int i = 0; i < _posRec.Count; i++)
            {
                recShadowData.PosData.Add(GM2DTools.ToProto(_posRec[i]));
            }
            for (int i = 0; i < _animRec.Count; i++)
            {
                recShadowData.AnimData.Add(GM2DTools.ToProto(_animRec[i]));
            }
            for (int i = 0; i < _idx2AnimName.Count; i++)
            {
                recShadowData.AnimNames.Add(_idx2AnimName[(byte) i]);
            }
            return recShadowData;
        }
    }
}