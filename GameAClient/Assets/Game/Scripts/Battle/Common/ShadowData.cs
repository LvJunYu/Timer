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

namespace GameA.Game
{
    [Serializable]
    // 影子数据，记录并回放上次游戏表现
    public class ShadowData
    {
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

        private List<IntVec2> _posRec = new List<IntVec2>();
        private List<AnimRec> _animRec = new List<AnimRec>();
        private Dictionary<string, byte> _animName2Idx = new Dictionary<string, byte>();
        private Dictionary<byte, string> _idx2AnimName = new Dictionary<byte, string>();
        private int _curAnimInx;
        private int _reviveFrame;
        private bool _isLiving;

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

        public void RecordClear()
        {
            _posRec.Clear();
            _animRec.Clear();
            _animName2Idx.Clear();
            _idx2AnimName.Clear();
        }

        public void PlayClear()
        {
            _reviveFrame = -1;
            _curAnimInx = 0;
            _isLiving = true;
        }

        public int Play(int frame)
        {
            if (frame >= _posRec.Count && ShadowUnit.Instance != null)
            {
                ShadowUnit.Instance.ShadowFinish();
                return 0;
            }
            if (ShadowUnit.Instance != null)
            {
                //更新动画
                while (_curAnimInx < _animRec.Count && frame >= _animRec[_curAnimInx].FrameIdx)
                {
                    switch (_animRec[_curAnimInx].NameIdx)
                    {
                        case 95:
                            ShadowUnit.Instance.OutPortal();
                            break;
                        case 96:
                            ShadowUnit.Instance.EnterPortal(frame);
                            break;
                        case 97:
                            ShadowUnit.Instance.Revive();
                            _reviveFrame = _animRec[_curAnimInx].FrameIdx;
                            //由于复活的那一帧没有记录位置，所以偏移再-1
                            _isLiving = true;
                            break;
                        case 98:
                            ShadowUnit.Instance.Dead(frame);
                            _isLiving = false;
                            break;
                        case 99:
                            ShadowUnit.Instance.ClearTrack(frame);
                            break;
                        case 100:
                            ShadowUnit.Instance.SetFacingDir(EMoveDirection.Left);
                            break;
                        case 101:
                            ShadowUnit.Instance.SetFacingDir(EMoveDirection.Right);
                            break;
                        default:
                            string animName;
                            if (_idx2AnimName.TryGetValue(_animRec[_curAnimInx].NameIdx, out animName))
                            {
                                ShadowUnit.Instance.UpdateAnim(animName, _animRec[_curAnimInx].Loop,
                                    _animRec[_curAnimInx].TimeScale * 0.01f, _animRec[_curAnimInx].TrackIdx);
                            }
                            break;
                    }
                    _curAnimInx++;
                }
                //更新位置
                if (_isLiving)
                {
                    //由于复活那一帧没有记录位置，需要使用下一帧的位置
                    if (_reviveFrame == frame)
                    {
                        ShadowUnit.Instance.UpdatePos(_posRec[frame + 1]);
                    }
                    else
                    {
                        ShadowUnit.Instance.UpdatePos(_posRec[frame]);
                    }
                }
            }
            return 1;
        }

        public void RecordPos(IntVec2 pos)
        {
            _posRec.Add(pos);
        }

        private string _lastAnimName;

        public void RecordAnimation(string animName, bool loop, float timeScale = 1f, int trackIdx = 0)
        {
            if (animName == _lastAnimName) return;
            _lastAnimName = animName;
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

        public void RecordDeath()
        {
            RecordAnim(98, false, 0, 0);
        }

        public void RecordRevive()
        {
            RecordAnim(97, false, 0, 0);
        }

        public void RecordEnterPortal()
        {
            RecordAnim(96, false, 0, 0);
        }

        public void RecordOutPortal()
        {
            RecordAnim(95, false, 0, 0);
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