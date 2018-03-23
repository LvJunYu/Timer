using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 8108, Type = typeof(Timer))]
    public class Timer : SwitchUnit
    {
        protected const string TimerFormat = "Timer_{0}";
        protected List<UnitBase> _units;
        protected SpriteRenderer[] _timerSprites;
        private bool _random;
        private bool _circulation;
        private int _second;
        private int _minSecond;
        private int _maxSecond;
        private int _timer;
        private int _endTimer;
        private bool _run;

        public override int SwitchTriggerId
        {
            get { return UnitDefine.TimerTriggerPressId; }
        }

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.CurScene.GetControlledUnits(_guid);
            OnActiveStateChanged();
        }

        protected override void Clear()
        {
            _timer = 0;
            _endTimer = 0;
            _run = false;
            ShowTime(false);
            base.Clear();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_run)
            {
                if (_timer > 0)
                {
                    _timer--;
                    ShowTime(true);
                }
                else
                {
                    if (_endTimer == 0)
                    {
                        DoControl();
                        _endTimer = 49;
                    }
                }

                if (_endTimer > 0)
                {
                    _endTimer--;
                    if (_endTimer == 0)
                    {
                        SetActiveState(EActiveState.Deactive);
                        if (_circulation)
                        {
                            SetActiveState(EActiveState.Active);
                            _run = _switchTrigger.Trigger == EActiveState.Active;
                        }
                    }
                }
            }
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _random = unitExtra.IsRandom;
            _circulation = unitExtra.TimerCirculation;
            _second = unitExtra.TimerSecond;
            _minSecond = unitExtra.TimerMinSecond;
            _maxSecond = unitExtra.TimerMaxSecond;
            return unitExtra;
        }

        public override void OnTriggerChanged(EActiveState value)
        {
            if (_endTimer > 0 || _eActiveState == EActiveState.Deactive)
            {
                return;
            }

            _run = value == EActiveState.Active;
        }

        protected override void OnActiveStateChanged()
        {
            if (_eActiveState == EActiveState.Active)
            {
                _timer = (_random ? GetRandomSecond() : _second) * ConstDefineGM2D.FixedFrameCount;
                _endTimer = 0;
                ShowTime(true);
                _run = _switchTrigger.Trigger == EActiveState.Active;
            }
            else if (_eActiveState == EActiveState.Deactive)
            {
                ShowTime(false);
                _run = false;
            }
        }

        internal override void OnObjectDestroy()
        {
            if (_timerSprites != null)
            {
                for (int i = 0; i < _timerSprites.Length; i++)
                {
                    Object.Destroy(_timerSprites[i].gameObject);
                }

                _timerSprites = null;
            }

            base.OnObjectDestroy();
        }

        private void ShowTime(bool show)
        {
            if (_view != null)
            {
                SetTimeRenderer(show, Mathf.CeilToInt(_timer / (float) ConstDefineGM2D.FixedFrameCount));
            }
        }

        private int GetRandomSecond()
        {
            return GameATools.GetRandomByValue(GameRun.Instance.LogicFrameCnt + _curPos.x + _curPos.y, _maxSecond, _minSecond);
        }

        private void DoControl()
        {
            if (_units != null && _switchTrigger != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnCtrlBySwitch();
                    }
                }
            }
        }

        private void SetTimeRenderer(bool show, int second)
        {
            if (_timerSprites == null)
            {
                _timerSprites = new SpriteRenderer[2];
                for (int i = 0; i < _timerSprites.Length; i++)
                {
                    var trans = new GameObject(string.Format(TimerFormat, i + 1)).transform;
                    CommonTools.SetParent(trans, _trans);
                    _timerSprites[i] = trans.gameObject.AddComponent<SpriteRenderer>();
                    _timerSprites[i].sortingOrder = (int) ESortingOrder.Item;
                    if (i == 0)
                    {
                        trans.localPosition = new Vector3(-0.4f, 0, -0.01f);
                    }
                    else
                    {
                        trans.localPosition = new Vector3(0, 0, -0.01f);
                    }
                }
            }

            for (int i = 0; i < _timerSprites.Length; i++)
            {
                _timerSprites[i].SetActiveEx(show);
                if (show)
                {
                    int value = i == 0 ? second / 10 : second % 10;
                    Sprite sprite;
                    if (JoyResManager.Instance.TryGetSprite(string.Format(TimerFormat, value), out sprite))
                    {
                        _timerSprites[i].sprite = sprite;
                    }
                }
            }
        }
    }
}