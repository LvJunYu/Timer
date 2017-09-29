using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class GUIInputCaptor : MonoBehaviour
    {
        public bool IsInputing;

        private void OnGUI()
        {
            if (IsInputing && Input.anyKeyDown)
            {
                Event e = Event.current;
                if (e.isKey)
                {
                    if (e.keyCode == KeyCode.Escape ||e.keyCode == KeyCode.Return)
                    {
                        Messenger<KeyCode>.Broadcast(EMessengerType.OnGetInputKeyCode, KeyCode.None);
                    }
                    else
                    {
                        Messenger<KeyCode>.Broadcast(EMessengerType.OnGetInputKeyCode, e.keyCode);
                    }
                }
                else if(e.isMouse)
                {
                    Messenger<KeyCode>.Broadcast(EMessengerType.OnGetInputKeyCode, KeyCode.None);
                }
            }
        }
    }
}