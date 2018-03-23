using System;
using GameA;
using GameA.Game;
using NewResourceSolution;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput.PlatformSpecific;


namespace UnityStandardAssets.CrossPlatformInput
{
    public enum EPlatform
    {
        Moblie,
        PC
    }

    public static class CrossPlatformInputManager
    {
        public static EPlatform Platform;
        private static VirtualInput virtualInput;

        static CrossPlatformInputManager()
        {
            if (RuntimeConfig.Instance.UseDebugMobileInput && Application.isEditor)
            {
                Platform = EPlatform.Moblie;
                virtualInput = new MobileInput();
                return;
            }
            if (Application.isEditor)
            {
                Platform = EPlatform.PC;
                virtualInput = new StandaloneInput();
                UpdateInputControlKeys();
            }
            else
            {
#if MOBILE_INPUT
                Platform = EPlatform.Moblie;
                virtualInput = new MobileInput();
#else
                Platform = EPlatform.PC;
                virtualInput = new StandaloneInput();
                UpdateInputControlKeys();
#endif
            }
        }

        public static void Update()
        {
            virtualInput.Update();
        }

        public static void RegisterVirtualAxis(VirtualAxis axis)
        {
            virtualInput.RegisterVirtualAxis(axis);
        }

        public static void RegisterVirtualButton(VirtualButton button)
        {
            virtualInput.RegisterVirtualButton(button);
        }

        public static void UnRegisterVirtualAxis(string _name)
        {
            if (_name == null)
            {
                throw new ArgumentNullException("_name");
            }
            virtualInput.UnRegisterVirtualAxis(_name);
        }

        public static void UnRegisterVirtualButton(string name)
        {
            virtualInput.UnRegisterVirtualButton(name);
        }

        // returns a reference to a named virtual axis if it exists otherwise null
        public static VirtualAxis VirtualAxisReference(string name)
        {
            return virtualInput.VirtualAxisReference(name);
        }

        // returns the platform appropriate axis for the given name
        public static float GetAxis(string name)
        {
            return GetAxis(name, false);
        }

        public static float GetAxisRaw(string name)
        {
            return GetAxis(name, true);
        }

        // private function handles both types of axis (raw and not raw)
        private static float GetAxis(string name, bool raw)
        {
            return virtualInput.GetAxis(name, raw);
        }

        // -- Button handling --
        public static bool GetButton(string name)
        {
            return virtualInput.GetButton(name);
        }

        public static bool GetButtonDown(string name)
        {
            return virtualInput.GetButtonDown(name);
        }

        public static bool GetButtonUp(string name)
        {
            return virtualInput.GetButtonUp(name);
        }

        public static void SetButtonDown(string name)
        {
            virtualInput.SetButtonDown(name);
        }

        public static void SetButtonUp(string name)
        {
            virtualInput.SetButtonUp(name);
        }

        public static void SetAxisPositive(string name)
        {
            virtualInput.SetAxisPositive(name);
        }

        public static void SetAxisNegative(string name)
        {
            virtualInput.SetAxisNegative(name);
        }

        public static void SetAxisZero(string name)
        {
            virtualInput.SetAxisZero(name);
        }

        public static void SetAxis(string name, float value)
        {
            virtualInput.SetAxis(name, value);
        }

        public static Vector3 mousePosition
        {
            get { return virtualInput.MousePosition(); }
        }

        public static void SetVirtualMousePositionX(float f)
        {
            virtualInput.SetVirtualMousePositionX(f);
        }

        public static void SetVirtualMousePositionY(float f)
        {
            virtualInput.SetVirtualMousePositionY(f);
        }

        public static void SetVirtualMousePositionZ(float f)
        {
            virtualInput.SetVirtualMousePositionZ(f);
        }

        public static void ClearVirtualInput()
        {
            virtualInput.UnRegisterAllVirtualButtonAndAxis();
            UpdateInputControlKeys();
        }

        public static void UpdateInputControlKeys()
        {
            for (int i = 0; i < (int) EInputKey.Max; i++)
            {
                EInputKey inputKey = (EInputKey) i;
                KeyCode keyCode;
                if (PlayerPrefs.HasKey(inputKey.ToString()))
                {
                    keyCode = (KeyCode) PlayerPrefs.GetInt(inputKey.ToString());
                }
                else
                {
                    keyCode = GetDefaultKey(inputKey);
                }
                ChangeInputControlKey(inputKey, keyCode);
            }
        }

        public static void ChangeInputControlKey(EInputKey eInputKey, KeyCode keyCode)
        {
            switch (eInputKey)
            {
                case EInputKey.Up:
                    SetButtonPositiveKey(InputManager.TagVertical, keyCode);
                    break;
                case EInputKey.Left:
                    SetButtonNegativeKey(InputManager.TagHorizontal, keyCode);
                    break;
                case EInputKey.Down:
                    SetButtonNegativeKey(InputManager.TagVertical, keyCode);
                    break;
                case EInputKey.Right:
                    SetButtonPositiveKey(InputManager.TagHorizontal, keyCode);
                    break;
                case EInputKey.Jump:
                    SetButtonPositiveKey(InputManager.TagJump, keyCode);
                    break;
                case EInputKey.Skill1:
                    SetButtonPositiveKey(InputManager.TagSkill1, keyCode);
                    break;
                case EInputKey.Skill2:
                    SetButtonPositiveKey(InputManager.TagSkill2, keyCode);
                    break;
                case EInputKey.Skill3:
                    SetButtonPositiveKey(InputManager.TagSkill3, keyCode);
                    break;
                case EInputKey.Assist:
                    SetButtonPositiveKey(InputManager.TagAssist, keyCode);
                    break;
                case EInputKey.PutObj:
                case EInputKey.DragScreen:
                case EInputKey.ScaleScreen:
                    break;
            }
        }

        public static KeyCode GetDefaultKey(EInputKey eInputKey)
        {
            switch (eInputKey)
            {
                case EInputKey.Up:
                    return KeyCode.W;
                case EInputKey.Left:
                    return KeyCode.A;
                case EInputKey.Down:
                    return KeyCode.S;
                case EInputKey.Right:
                    return KeyCode.D;
                case EInputKey.Jump:
                    return KeyCode.Space;
                case EInputKey.Skill1:
                    return KeyCode.J;
                case EInputKey.Skill2:
                    return KeyCode.K;
                case EInputKey.Skill3:
                    return KeyCode.L;
                case EInputKey.Assist:
                    return KeyCode.E;
                case EInputKey.PutObj:
                    return KeyCode.Mouse0;
                case EInputKey.DragScreen:
                    return KeyCode.Mouse1;
                case EInputKey.ScaleScreen:
                    return KeyCode.Mouse2;
                default:
                    return KeyCode.None;
            }
        }

        public static KeyCode GetButtonPositiveKey(string name)
        {
            return virtualInput.GetButtonPositiveKey(name);
        }

        public static void SetButtonPositiveKey(string name, KeyCode positiveKey)
        {
            virtualInput.SetButtonPositiveKey(name, positiveKey);
        }

        public static void SetButtonNegativeKey(string name, KeyCode negativeKey)
        {
            virtualInput.SetButtonNegativeKey(name, negativeKey);
        }

        // virtual axis and button classes - applies to mobile input
        // Can be mapped to touch joysticks, tilt, gyro, etc, depending on desired implementation.
        // Could also be implemented by other input devices - kinect, electronic sensors, etc
        public class VirtualAxis
        {
            public string name { get; private set; }
            private float m_Value;
            public bool matchWithInputManager { get; private set; }


            public VirtualAxis(string name) : this(name, true)
            {
            }


            public VirtualAxis(string name, bool matchToInputSettings)
            {
                this.name = name;
                matchWithInputManager = matchToInputSettings;
                //RegisterVirtualAxis(this);
            }


            // removes an axes from the cross platform input system
            public void Remove()
            {
                UnRegisterVirtualAxis(name);
            }


            // a controller gameobject (eg. a virtual thumbstick) should update this class
            public void Update(float value)
            {
                m_Value = value;
            }


            public float GetValue
            {
                get { return m_Value; }
            }


            public float GetValueRaw
            {
                get { return m_Value; }
            }
        }

        // a controller gameobject (eg. a virtual GUI button) should call the
        // 'pressed' function of this class. Other objects can then read the
        // Get/Down/Up state of this button.
        public class VirtualButton
        {
            public string name { get; private set; }
            private int lastPressedFrame = -5;
            private int releasedFrame = -5;
            private bool pressed;

            public bool matchWithInputManager { get; private set; }

            //为了PC玩家自定义按键，这里设置按键
            public KeyCode PositiveKey;

            public KeyCode NegativeKey;

            public VirtualButton(string name) : this(name, true)
            {
            }

            public VirtualButton(string name, bool matchToInputSettings)
            {
                //默认配置
                switch (name)
                {
                    case InputManager.TagJump:
                        PositiveKey = KeyCode.Space;
                        break;
                    case InputManager.TagSkill1:
                        PositiveKey = KeyCode.J;
                        break;
                    case InputManager.TagSkill2:
                        PositiveKey = KeyCode.K;
                        break;
                    case InputManager.TagSkill3:
                        PositiveKey = KeyCode.L;
                        break;
                    case InputManager.TagAssist:
                        PositiveKey = KeyCode.E;
                        break;
                    case InputManager.TagHorizontal:
                        PositiveKey = KeyCode.D;
                        NegativeKey = KeyCode.A;
                        break;
                    case InputManager.TagVertical:
                        PositiveKey = KeyCode.W;
                        NegativeKey = KeyCode.S;
                        break;
                }
                this.name = name;
                matchWithInputManager = matchToInputSettings;
                //  RegisterVirtualButton(this);
            }

            // A controller gameobject should call this function when the button is pressed down
            public void Pressed()
            {
                if (pressed)
                {
                    return;
                }
                pressed = true;
                lastPressedFrame = Time.frameCount;
            }

            // A controller gameobject should call this function when the button is released
            public void Released()
            {
                pressed = false;
                releasedFrame = Time.frameCount;
            }

            // the controller gameobject should call Remove when the button is destroyed or disabled
            public void Remove()
            {
                UnRegisterVirtualButton(name);
            }

            // these are the states of the button which can be read via the cross platform input system
            public bool GetButton
            {
                get { return pressed; }
            }

            public bool GetButtonDown
            {
                get { return lastPressedFrame - Time.frameCount == 0; }
            }

            public bool GetButtonUp
            {
                get { return (releasedFrame == Time.frameCount - 0); }
            }
        }
    }
}