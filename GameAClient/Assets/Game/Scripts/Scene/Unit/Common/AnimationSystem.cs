/********************************************************************
** Filename : AnimationSystem
** Author : cwc
** Date : 2016/10/14 星期日 下午 7:08:27
** Summary : AnimationSystem
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using Spine;
using Spine.Unity;
using Animation = Spine.Animation;
using AnimationState = Spine.AnimationState;

namespace GameA.Game
{
    [Serializable]
    public class AnimationSystem
    {
        private SkeletonAnimation _skeletonAnimation;
        private readonly string[] _currentAnimation = new string[4];
        private readonly Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();
		// 默认动作，初始状态时播放
        private string _initAniName;
		/// <summary>
		/// 动画事件回调
		/// </summary>
		private Dictionary<string, Action> _eventHandles = new Dictionary<string, Action> ();

        public AnimationSystem(SkeletonAnimation skeletonAnimation)
        {
            _skeletonAnimation = skeletonAnimation;
        }

        public void Set()
        {
            Animation[] animations = _skeletonAnimation.state.Data.skeletonData.animations.Items;
            for (int i = 0; i < animations.Length; i++)
            {
                _animations.Add(animations[i].Name, animations[i]);
            }
            _skeletonAnimation.state.Event += OnEvent;
        }

        internal void OnFree()
        {
            _animations.Clear();
            _eventHandles.Clear();
            _initAniName = null;
            for (int i = 0; i < _currentAnimation.Length; i++)
            {
                _currentAnimation[i] = null;
            }
        }

        public bool Init(string aniName)
        {
            if (!string.IsNullOrEmpty(aniName))
            {
                _initAniName = aniName;
                PlayLoop(_initAniName);
            }
            return true;
        }

        private TrackEntry SetAnimation(string aniName, int trackIndex = 0, float timeScale = 1, bool loop = false)
        {
            if (!IsPlaying(aniName, trackIndex))
            {
//				UnityEngine.Debug.Log ("____________________ SetAnimiation " + aniName + " trackId: " + trackIndex + " ts: " + timeScale + " loop: " + loop);
                _currentAnimation[trackIndex] = aniName;
            }
            else if (_skeletonAnimation == null)
            {
                return null;
            } 
            else if (Util.IsFloatEqual(timeScale, _skeletonAnimation.state.TimeScale))
            {
                return null;
            }
            Animation ani;
            if (!_animations.TryGetValue(aniName, out ani))
            {
                LogHelper.Error("animation is not exist. {0}", aniName);
                return null;
            }
            _skeletonAnimation.state.TimeScale = timeScale;
            var entry = _skeletonAnimation.state.SetAnimation(trackIndex, ani, loop);
            if (!loop)
            {
                entry.Complete += entry_Complete;
            }
            return entry;
        }

        void entry_Complete(AnimationState state, int trackIndex, int loopCount)
        {
            _currentAnimation[trackIndex] = null;
        }

		/// <summary>
		/// 注册的动画事件回调
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="trackIndx">Track indx.</param>
		/// <param name="e">E.</param>
		void OnEvent (AnimationState state, int trackIndx, Event e) {
//			UnityEngine.Debug.Log ("____________________________onSpineEvent " + e.Data.name);
			Action handle = null;
			if (_eventHandles.TryGetValue (e.Data.name, out handle)) {
				handle.Invoke ();
			}
		}

        public TrackEntry PlayOnce(string aniName, float timeScale = 1, int trackIndex = 0)
        {
            return SetAnimation(aniName, trackIndex, timeScale);
        }

        public bool PlayLoop(string aniName, float timeScale = 1, int trackIndex = 0)
        {
            if (SetAnimation(aniName, trackIndex, timeScale, true) == null)
            {
                return false;
            }
            return true;
        }

        public bool IsPlaying(string aniName, int trackIndex = 0)
        {
            if (_currentAnimation[trackIndex] == aniName)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            if (_skeletonAnimation != null)
            {
                if (_skeletonAnimation.state != null)
                {
                    _skeletonAnimation.state.ClearTracks();
                }
                if (_skeletonAnimation.Skeleton != null)
                {
                    _skeletonAnimation.Skeleton.SetToSetupPose();
                    _skeletonAnimation.Update(ConstDefineGM2D.FixedDeltaTime);
                }
            }
            for (int i = 0; i < _currentAnimation.Length; i++)
            {
                _currentAnimation[i] = null;
            }
            if (_initAniName != null)
            {
                PlayLoop(_initAniName);
            }
        }

        public void ClearTrack(int idx)
        {
            if (_skeletonAnimation != null)
            {
                _skeletonAnimation.state.ClearTrack(idx);
            }
            _currentAnimation[idx] = null;
        }

		/// <summary>
		/// 添加动画事件回调
		/// </summary>
		/// <returns><c>true</c>, if event handle was added, <c>false</c> otherwise.</returns>
		/// <param name="eventName">Event name.</param>
		/// <param name="handle">Handle.</param>
		public bool AddEventHandle(string eventName, Action handle)
		{
		    if (_eventHandles.ContainsKey(eventName))
		    {
		        LogHelper.Warning("Already contains event name {0} when add animation event on {1}", eventName,
		            _skeletonAnimation.name);
		        return false;
		    }
		    _eventHandles[eventName] = handle;
		    return true;
		}
    }
}

