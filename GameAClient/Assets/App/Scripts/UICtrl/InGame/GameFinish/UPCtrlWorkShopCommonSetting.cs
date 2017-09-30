﻿using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UPCtrlWorkShopCommonSetting : UPCtrlInputKeysSettingBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private USCtrlGameSettingItem _playBGMusic_2;
        private USCtrlGameSettingItem _playSoundsEffects_2;
        private List<Dropdown.OptionData> _optionDatas;
  
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            if ((int) EInputKey.Max != _cachedView.UsInputKeyViews.Length)
            {
                LogHelper.Error("EInputKey.Max != _cachedView.UMInputKeyViews.Length");
                return;
            }
            _usCtrls = new USCtrlInputKeySetting[(int) EInputKey.Max];
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i] = new USCtrlInputKeySetting();
                _usCtrls[i].Init(_cachedView.UsInputKeyViews[i]);
                _usCtrls[i].InitInputKey((EInputKey) i); //注意：枚举中的排序必须和数组中排序相同
                _usCtrls[i].AddBtnCallBack(StartSettingInputKey);
            }
            _cachedView.SureBtn_2.onClick.AddListener(OnOKBtn);
            _cachedView.SureBtn_3.onClick.AddListener(OnOKBtn);
            _cachedView.RestoreDefaultBtn.onClick.AddListener(OnRestoreDefaultBtn);
            _playBGMusic_2 = new USCtrlGameSettingItem();
            _playBGMusic_2.Init(_cachedView.PlayBackGroundMusic_2);
            _playSoundsEffects_2 = new USCtrlGameSettingItem();
            _playSoundsEffects_2.Init(_cachedView.PlaySoundsEffects_2);
            _cachedView.FullScreenToggle.onValueChanged.AddListener(OnFullScreenToggleValueChanged);
            _cachedView.ResolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownValueChanged);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.CommonSettingPanel.SetActive(true);
            UpdateSettingItem();
            UpdateScreenSettingView();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.CommonSettingPanel.SetActive(false);
        }

        private void UpdateSettingItem()
        {
            _playBGMusic_2.SetData(GameSettingData.Instance.PlayMusic, _mainCtrl.OnClickMusicButton);
            _playSoundsEffects_2.SetData(GameSettingData.Instance.PlaySoundsEffects, _mainCtrl.OnClickSoundsEffectsButton);
        }
        
        private void OnResolutionDropdownValueChanged(int arg0)
        {
            ScreenResolutionManager.Instance.SetResolution(arg0);
        }

        private void OnFullScreenToggleValueChanged(bool arg0)
        {
            SetFullScreen(arg0);
        }

        private void SetFullScreen(bool value)
        {
            ScreenResolutionManager.Instance.SetFullScreen(value);
        }

        private void UpdateScreenSettingView()
        {
            bool fullScreen = ScreenResolutionManager.Instance.FullScreen;
            _cachedView.WindowScreenToggle.isOn = !fullScreen;
            _cachedView.FullScreenToggle.isOn = fullScreen;
            OnFullScreenToggleValueChanged(fullScreen);
            InitOptions(ScreenResolutionManager.Instance.AllResolutions);
            _cachedView.ResolutionDropdown.value = ScreenResolutionManager.Instance.CurResolutionIndex;
        }

        private void InitOptions(List<Resolution> resolutions)
        {
            if (_optionDatas != null) return;
            _optionDatas = new List<Dropdown.OptionData>(resolutions.Count);
            for (int i = 0; i < resolutions.Count; i++)
            {
                _optionDatas.Add(new Dropdown.OptionData(ResolutionToString(resolutions[i])));
            }
            _cachedView.ResolutionDropdown.options = _optionDatas;
        }

        private string ResolutionToString(Resolution resolution)
        {
            return string.Format("{0}×{1}", resolution.width, resolution.height);
        }
    }
}