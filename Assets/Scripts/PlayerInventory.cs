using UnityEngine;

namespace VanzAI
{
    /// <summary>
    /// 플레이어의 아이템 및 상태를 관리하는 클래스.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        private static PlayerInventory _instance;
        public static PlayerInventory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindAnyObjectByType<PlayerInventory>();
                }
                return _instance;
            }
        }

        [Header("Inventory State")]
        public bool hasKey = false;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            _instance = this;
        }

        public void AddKey()
        {
            hasKey = true;
            Debug.Log("[PlayerInventory] Key added to inventory.");
        }
    }
}
