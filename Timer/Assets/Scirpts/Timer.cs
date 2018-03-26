using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private const string HourStr = "时";
    private const string MinuteStr = "分";
    private const string SecondStr = "秒";
    public Text LeftTimeTxt;
    public Button StartBtn;
    public Button PauseBtn;
    public Button ResetBtn;
    public Slider Slider;
    private int _interval;
    private float _timer;
    private bool _run;
    private StringBuilder _stringBuilder = new StringBuilder();

    private void Awake()
    {
        StartBtn.onClick.AddListener(() => { _run = true; });
        PauseBtn.onClick.AddListener(() => { _run = false; });
        ResetBtn.onClick.AddListener(Reset);
        Slider.onValueChanged.AddListener(value =>
        {
            int seconds = (int) value;
            _interval = GetSecond(seconds);
            Reset();
        });
    }

    private void Reset()
    {
        _run = false;
        _timer = 0;
        RefreshLeftTimeTxt(false);
    }

    private void Start()
    {
        _timer = 0;
        _interval = 5;
        _run = true;
//        _interval = GetSecond(Slider.value);
    }

    private void Update()
    {
        if (_run)
        {
            _timer += Time.deltaTime;
            if (_timer >= _interval)
            {
                Messagebox.MessageBox(IntPtr.Zero, "该喝水了！", "亲爱的", 0);
                _timer -= _interval;
            }

            RefreshLeftTimeTxt(true);
        }
    }

    private void RefreshLeftTimeTxt(bool showSecond)
    {
        LeftTimeTxt.text = SecondToHour(_interval - _timer, showSecond);
    }

    private string SecondToHour(float seconds, bool showSecond)
    {
        int hour;
        int minute;
        int second = (int) seconds;

        minute = second / 60;
        second = second % 60;
        hour = minute / 60;
        minute = minute % 60;

        _stringBuilder.Length = 0;
        bool start = false;
        if (hour > 0)
        {
            _stringBuilder.Append(hour);
            _stringBuilder.Append(HourStr);
            start = true;
        }

        if (start || minute > 0)
        {
            _stringBuilder.Append(minute);
            _stringBuilder.Append(MinuteStr);
        }

        if (showSecond)
        {
            _stringBuilder.Append(second);
            _stringBuilder.Append(SecondStr);
        }

        return _stringBuilder.ToString();
    }

    private class Messagebox
    {
        [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr handle, String message, String title, int type);
    }

    private int GetSecond(float value)
    {
        return (int) (value * 60);
    }
}