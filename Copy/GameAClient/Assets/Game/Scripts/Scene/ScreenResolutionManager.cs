using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class ScreenResolutionManager
    {
        private static ScreenResolutionManager _instance;

        public static ScreenResolutionManager Instance
        {
            get { return _instance ?? (_instance = new ScreenResolutionManager()); }
        }

        private List<Resolution> _allResolutions = new List<Resolution>(10);
        private List<Resolution> _fullScreenResolutions = new List<Resolution>(1);
        private Resolution _curResolution;
        private int _curResolutionIndex;
        private bool _fullScreen;
        private bool _beyondBoard;

        private const string FullScreenTag = "FullScreenTag";
        private bool _selectFullScreen;

        private int _selectIndex;

        public Resolution CurRealResolution
        {
            get { return _curResolution; }
        }

        public List<Resolution> AllResolutionOptions
        {
            get
            {
                if (_selectFullScreen)
                {
                    return _fullScreenResolutions;
                }
                return _allResolutions;
            }
        }

        public bool FullScreen
        {
            get { return _fullScreen; }
        }

        public int SelectIndex
        {
            get { return _selectIndex; }
        }

        private ScreenResolutionManager()
        {
            GetAllResolutions();
            Load();
            ClearChange();
        }

        private void GetAllResolutions()
        {
            _fullScreenResolutions.Add(Screen.currentResolution);
            var resolutions = Screen.resolutions;
            for (int i = 0; i < resolutions.Length; i++)
            {
                int width = resolutions[i].width;
                int height = resolutions[i].height;
                //分辨率高度必须小于屏幕分辨率高度，否则窗口会超出屏幕
                if (!CheckResolution(width, height))
                {
                    continue;
                }
                bool canAdd = false;
                if (width / (float) 16 == height / (float) 9 || width / (float) 16 == height / (float) 10)
                {
                    canAdd = true;
                    //检查是否重复
                    for (int j = 0; j < _allResolutions.Count; j++)
                    {
                        if (CheckSameResolution(resolutions[i], _allResolutions[j]))
                        {
                            canAdd = false;
                            break;
                        }
                    }
                }
                if (canAdd)
                {
                    _allResolutions.Add(resolutions[i]);
                }
            }
            _allResolutions.Sort((p, q) => p.width * 1000 + p.height - q.width * 1000 - q.height);
        }

        private void Load()
        {
            if (PlayerPrefs.HasKey(FullScreenTag))
            {
                _fullScreen = PlayerPrefs.GetInt(FullScreenTag) != 0;
            }
            else
            {
                _fullScreen = false;
            }
            if (_fullScreen)
            {
                _curResolutionIndex = 0;
                _curResolution = _fullScreenResolutions[0];
                return;
            }
            _curResolution = new Resolution();
            //设置默认分辨率
            _curResolution.width = Screen.width;
            _curResolution.height = Screen.height;
            _curResolutionIndex = IndexOfResolutions(_curResolution, _allResolutions);
            if (_curResolutionIndex >= 0)
            {
                _curResolution = _allResolutions[_curResolutionIndex];
            }
            //若列表中没有，且不会超出屏幕，则添加
            else if (CheckResolution(_curResolution.width, _curResolution.height))
            {
                _allResolutions.Add(_curResolution);
                _allResolutions.Sort((p, q) => p.width * 1000 + p.height - q.width * 1000 - q.height);
                _curResolutionIndex = _allResolutions.IndexOf(_curResolution);
            }
            else
            {
                _beyondBoard = true;
            }
        }

        private bool CheckResolution(int width, int height)
        {
            return width <= Screen.currentResolution.width && height <= Screen.currentResolution.height - 80;
        }

        //处理默认分辨率比屏幕分辨率小的情况
        public void Init()
        {
            if (_beyondBoard)
            {
                _beyondBoard = false;
                if (_allResolutions.Count > 0)
                {
                    _curResolutionIndex = _allResolutions.Count - 1;
                    SetResolution(_allResolutions[_curResolutionIndex], false);
                    return;
                }
                //处理比支持的分辨率还小的情况
                int height = (Screen.currentResolution.height - 100) / 90 * 90;
                int width = height / 9 * 16;
                if (width > Screen.currentResolution.width)
                {
                    width = Screen.currentResolution.width / 160 * 160;
                    height = width / 16 * 9;
                }
                Resolution resolution = new Resolution();
                resolution.width = width;
                resolution.height = height;
                _allResolutions.Add(resolution);
                _allResolutions.Sort((p, q) => p.width * 1000 + p.height - q.width * 1000 - q.height);
                _curResolutionIndex = _allResolutions.IndexOf(resolution);
                SetResolution(resolution, false);
                ClearChange();
            }
        }

        public void Save()
        {
            bool needSave = false;
            if (_selectFullScreen != _fullScreen)
            {
                needSave = true;
                _fullScreen = _selectFullScreen;
                PlayerPrefs.SetInt(FullScreenTag, _fullScreen ? 1 : 0);
                if (_selectFullScreen)
                {
                    _curResolution = _fullScreenResolutions[0];
                    _curResolutionIndex = 0;
                }
                else
                {
                    _curResolution = _allResolutions[_selectIndex];
                    _curResolutionIndex = _selectIndex;
                }
            }
            else if (!_selectFullScreen && _selectIndex != _curResolutionIndex)
            {
                needSave = true;
                _curResolutionIndex = _selectIndex;
                _curResolution = _allResolutions[_curResolutionIndex];
            }
            if (needSave)
            {
                SetResolution(_curResolution, _fullScreen);
            }
        }

        public void ClearChange()
        {
            _selectIndex = _curResolutionIndex;
            _selectFullScreen = _fullScreen;
        }

        public void SetFullScreen(bool value)
        {
            _selectFullScreen = value;
            if (_selectFullScreen)
            {
                _selectIndex = 0;
            }
            else
            {
                _selectIndex = _curResolutionIndex;
            }
        }

        public void SetResolution(Resolution resolution, bool fullScreen)
        {
            _curResolution = resolution;
            _fullScreen = fullScreen;
            Screen.SetResolution(resolution.width, resolution.height, fullScreen);
            Screen.fullScreen = fullScreen;
        }

        public void SetResolution(int index)
        {
            if (index >= _allResolutions.Count)
            {
                LogHelper.Error("resolutionIndex > _allResolutions.Count");
                return;
            }
            _selectIndex = index;
//            _curResolutionIndex = index;
//            SetResolution(_allResolutions[index], _fullScreen);
        }

        private bool CheckSameResolution(Resolution r1, Resolution r2)
        {
            return r1.width == r2.width && r1.height == r2.height;
        }

        private int IndexOfResolutions(Resolution r, List<Resolution> resolutions)
        {
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (CheckSameResolution(resolutions[i], r))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}