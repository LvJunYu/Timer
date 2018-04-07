using UnityEngine;

namespace GameA.Game
{
    public class TextBar : MonoBehaviour
    {
        public TextMesh Content;

        private Transform _trans;

        private void Awake()
        {
            _trans = transform;
        }

        public void SetContent(string content)
        {
            Content.text = content;
        }

        void Update()
        {
            _trans.rotation = Quaternion.identity;
        }

        public void SetEnable(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}