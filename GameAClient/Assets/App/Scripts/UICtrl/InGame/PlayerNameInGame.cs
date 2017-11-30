using UnityEngine;

namespace GameA.Game
{
    public class PlayerNameInGame : MonoBehaviour
    {
        public GameObject Root;
        public TextMesh TextMesh;
        private Transform _trans;

        void Awake()
        {
            _trans = transform;
            TextMesh.GetComponent<Renderer>().sortingOrder = (int) ESortingOrder.DragingItem;
            SetNameActive(false);
        }

        void Update()
        {
            _trans.rotation = Quaternion.identity;
        }

        public void SetName(string name)
        {
            TextMesh.text = name;
        }

        public void SetNameActive(bool value)
        {
            if (Root.activeSelf != value)
            {
                Root.SetActive(value);
            }
        }
    }
}