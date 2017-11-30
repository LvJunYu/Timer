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
            _trans.rotation = Quaternion.identity;
            TextMesh.GetComponent<Renderer>().sortingOrder = (int) ESortingOrder.DragingItem;
            SetNameActive(false);
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