using GameA.Game;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific
{
    public class StandaloneInput : VirtualInput
    {
        // 需要监听的输入按钮
        private string[] _virtualBtnNames =
        {
            InputManager.TagJump, InputManager.TagSkill1, InputManager.TagSkill2, InputManager.TagSkill3,
            InputManager.TagAssist, InputManager.TagHorizontal, InputManager.TagVertical
        };

        private void AddButton(string name)
        {
            // we have not registered this button yet so add it, happens in the constructor
            CrossPlatformInputManager.RegisterVirtualButton(new CrossPlatformInputManager.VirtualButton(name));
        }

        public override float GetAxis(string name, bool raw)
        {
//            return raw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
            if (!virtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
            var PositiveKey = virtualButtons[name].PositiveKey;
            var NegativeKey = virtualButtons[name].NegativeKey;
            if (Input.GetKey(PositiveKey))
                return 1;
            if (Input.GetKey(NegativeKey))
                return -1;
            return 0;
        }

        public override bool GetButton(string name)
        {
            //return Input.GetButton(name);
            if (virtualButtons.ContainsKey(name))
            {
                return virtualButtons[name].GetButton;
            }

            AddButton(name);
            return virtualButtons[name].GetButton;
        }

        public override void SetButtonPositiveKey(string name, KeyCode positiveKey)
        {
            if (!virtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
            virtualButtons[name].PositiveKey = positiveKey;
        }

        public override void SetButtonNegativeKey(string name, KeyCode negativeKey)
        {
            if (!virtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
            virtualButtons[name].NegativeKey = negativeKey;
        }

        public override KeyCode GetButtonPositiveKey(string name)
        {
            if (!virtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
           return virtualButtons[name].PositiveKey;
        }

        public override bool GetButtonDown(string name)
        {
            //return Input.GetButtonDown(name);
            if (virtualButtons.ContainsKey(name))
            {
                return virtualButtons[name].GetButtonDown;
            }

            AddButton(name);
            return virtualButtons[name].GetButtonDown;
        }


        public override bool GetButtonUp(string name)
        {
            //return Input.GetButtonUp(name);
            if (virtualButtons.ContainsKey(name))
            {
                return virtualButtons[name].GetButtonUp;
            }

            AddButton(name);
            return virtualButtons[name].GetButtonUp;
        }


        public override void SetButtonDown(string name)
        {
//            throw new Exception(
//                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetButtonUp(string name)
        {
//            throw new Exception(
//                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxisPositive(string name)
        {
//            throw new Exception(
//                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxisNegative(string name)
        {
//            throw new Exception(
//                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxisZero(string name)
        {
//            throw new Exception(
//                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override void SetAxis(string name, float value)
        {
//            throw new Exception(
//                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }


        public override Vector3 MousePosition()
        {
            return Input.mousePosition;
        }

        public override void Update()
        {
            for (int i = 0; i < _virtualBtnNames.Length; i++)
            {
                if (!virtualButtons.ContainsKey(_virtualBtnNames[i]))
                {
                    AddButton(_virtualBtnNames[i]);
                }
                if (Input.GetKeyDown(virtualButtons[_virtualBtnNames[i]].PositiveKey) ||
                    Input.GetKeyDown(virtualButtons[_virtualBtnNames[i]].NegativeKey))
                {
                    virtualButtons[_virtualBtnNames[i]].Pressed();
                }
                if (Input.GetKeyUp(virtualButtons[_virtualBtnNames[i]].PositiveKey) ||
                    Input.GetKeyUp(virtualButtons[_virtualBtnNames[i]].NegativeKey))
                {
                    virtualButtons[_virtualBtnNames[i]].Released();
                }
            }
        }
    }
}