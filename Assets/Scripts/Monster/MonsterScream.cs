using UnityEngine;
using VanzAI.Managers;

namespace VanzAI.Monster
{
    public class MonsterScream : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioClip screamClip;
        
        private Transform _playerTransform;
        private bool _hasScreamedThisChase = false;
        private MonsterChase _monsterChase;
        private float _detectionRange;

        private void Awake()
        {
            _monsterChase = GetComponent<MonsterChase>();
        }

        private void Start()
        {
            if (_monsterChase != null)
            {
                _playerTransform = _monsterChase.playerTransform;
                _detectionRange = _monsterChase.detectionRange;
            }

            if (_playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) _playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            float distance = Vector3.Distance(transform.position, _playerTransform.position);

            if (distance <= _detectionRange)
            {
                if (!_hasScreamedThisChase)
                {
                    PlayScream();
                    _hasScreamedThisChase = true;
                }
            }
            else
            {
                // Reset scream when player is out of range
                _hasScreamedThisChase = false;
            }
        }

        private void PlayScream()
        {
            if (screamClip == null) return;
            AudioManager.Instance.PlaySFX(screamClip, transform.position);
            Debug.Log("[MonsterScream] SCREAM!");
        }
    }
}
