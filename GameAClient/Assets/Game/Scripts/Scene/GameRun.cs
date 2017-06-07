/********************************************************************
** Filename : GameRun
** Author : Dong
** Date : 2017/6/7 星期三 下午 2:28:19
** Summary : GameRun
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class GameRun
    {
        public static GameRun _instance;

        public static GameRun Instance
        {
            get { return _instance ?? (_instance = new GameRun()); }
        }

        private bool _executeLogic;
        private int _logicFrameCnt;
        private float _unityTimeSinceGameStarted;

        public void Update()
        {
            CrossPlatformInputManager.Update();
            if (MapManager.Instance == null || !MapManager.Instance.GenerateMapComplete)
            {
                return;
            }
            GuideManager.Instance.Update();

            _unityTimeSinceGameStarted += Time.deltaTime * GM2DGame.Instance.GamePlaySpeed;
            while (_logicFrameCnt * ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted)
            {
                UpdateRenderer(Mathf.Min(Time.deltaTime, ConstDefineGM2D.FixedDeltaTime));

                if (_logicFrameCnt * ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted)
                {
                    UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
                    _logicFrameCnt++;
                }
            }
        }

        private void UpdateRenderer(float deltaTime)
        {
        }

        private void UpdateLogic(float deltaTime)
        {
        }
    }
}
