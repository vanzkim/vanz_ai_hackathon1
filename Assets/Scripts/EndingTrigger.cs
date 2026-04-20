using UnityEngine;

namespace VanzAI.Triggers
{
    /// <summary>
    /// 플레이어가 닿았을 때 엔딩 씬으로 전환하는 트리거.
    /// </summary>
    public class EndingTrigger : MonoBehaviour
    {
        private bool _isTransitioning = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_isTransitioning) return;
            if (!IsPlayer(other)) return;

            _isTransitioning = true;
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.LoadScene(GameSceneManager.SceneType.Ending);
                Debug.Log("[EndingTrigger] Player entered! Loading EndingScene.");
            }
            else
            {
                Debug.LogError("[EndingTrigger] GameSceneManager instance not found!");
            }
        }

        /// <summary>
        /// Player 태그이거나, 이름이 Player_Model / Player_Cutscene 인 경우 플레이어로 인정.
        /// </summary>
        private static bool IsPlayer(Collider other)
        {
            if (other.CompareTag(VanzConstants.PlayerTag)) return true;

            string rootName = other.transform.root.name;
            if (other.name == VanzConstants.PlayerCutsceneName ||
                rootName == VanzConstants.PlayerCutsceneName) return true;

            if (other.name == VanzConstants.PlayerModelName ||
                rootName == VanzConstants.PlayerModelName) return true;

            return false;
        }
    }
}
