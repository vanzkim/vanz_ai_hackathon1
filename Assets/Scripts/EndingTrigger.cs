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

            // 'Player' 태그를 가진 오브젝트거나 이름이 Player_Cutscene 인 경우 체크
            if (other.CompareTag("Player") || other.name == "Player_Cutscene" || other.transform.root.name == "Player_Cutscene")
            {
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
        }
    }
}
