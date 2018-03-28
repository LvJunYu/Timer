using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private const string HourStr = "时";
    private const string MinuteStr = "分";
    private const string SecondStr = "秒";
    private const string IntervalKey = "interval";
    public Text LeftTimeTxt;
    public Text StartTxt;
    public Button StartBtn;
    public Button ResetBtn;
    public Button LeftBtn;
    public Button RightBtn;
    public Slider Slider;
    private int _interval;
    private float _timer;
    private bool _run;
    private StringBuilder _stringBuilder = new StringBuilder();

    private void Awake()
    {
        StartBtn.onClick.AddListener(() =>
        {
            SetRun(!_run);
            PlayerPrefs.SetInt(IntervalKey, (int) Slider.value);
        });
        ResetBtn.onClick.AddListener(Reset);
        LeftBtn.onClick.AddListener(() =>
        {
            var curValue = Slider.value;
            if (curValue % 10 == 0)
            {
                curValue -= 10;
            }
            else
            {
                curValue = (int) (curValue / 10) * 10;
            }

            Slider.value = Mathf.Clamp(curValue, Slider.minValue, Slider.maxValue);
        });
        RightBtn.onClick.AddListener(() =>
        {
            var curValue = Slider.value;
            Slider.value = Mathf.Clamp((int) (curValue / 10 + 1) * 10, Slider.minValue, Slider.maxValue);
        });
        Slider.onValueChanged.AddListener(value =>
        {
            int seconds = (int) value;
            _interval = GetSecond(seconds);
            Reset();
        });
    }

    private void SetRun(bool value)
    {
        _run = value;
        if (value)
        {
            StartTxt.text = "暂 停";
        }
        else
        {
            StartTxt.text = "开 始";
        }
    }

    private void Reset()
    {
        SetRun(false);
        _timer = 0;
        RefreshLeftTimeTxt(false);
    }

    private void Start()
    {
        _timer = 0;
        if (PlayerPrefs.HasKey(IntervalKey))
        {
            Slider.value = PlayerPrefs.GetInt(IntervalKey);
        }
        else
        {
            Slider.value = 30;
        }

        SetRun(true);
    }

    private void Update()
    {
        if (_run)
        {
            _timer += Time.deltaTime;
            if (_timer >= _interval)
            {
                SetRun(false);
                _timer = 0;
                LeftTimeTxt.text = "亲爱的，该喝水了~";
                WinTools.ShowForward();
            }
            else
            {
                RefreshLeftTimeTxt(true);
            }
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

        if (showSecond || second > 0)
        {
            _stringBuilder.Append(second);
            _stringBuilder.Append(SecondStr);
        }

        return _stringBuilder.ToString();
    }

    private int GetSecond(float value)
    {
        return (int) (value * 60);
    }
}