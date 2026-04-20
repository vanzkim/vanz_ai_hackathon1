using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;

namespace VanzAI.Sequences
{
    /// <summary>
    /// 인트로 컷씬이 끝난 후 컷씬용 플레이어를 비활성화하고,
    /// 실제 조작 가능한 플레이어 모델을 활성화하며 카메라를 연결합니다.
    /// </summary>
    public class IntroSequenceManager : MonoBehaviour
    {
        [Header("Timeline")]
        [SerializeField] private PlayableDirector introDirector;

        [Header("Players")]
        [SerializeField] private GameObject cutscenePlayer;
        [SerializeField] private GameObject gameplayPlayer;

        [Header("Camera")]
        [SerializeField] private CinemachineCamera playerCamera;

        private void Awake()
        {
            if (introDirector == null)
                introDirector = GetComponent<PlayableDirector>();
        }

        private void OnEnable()
        {
            if (introDirector != null)
                introDirector.stopped += OnIntroFinished;
        }

        private void OnDisable()
        {
            if (introDirector != null)
                introDirector.stopped -= OnIntroFinished;
        }

        private void OnIntroFinished(PlayableDirector director)
        {
            Debug.Log("[IntroSequenceManager] Intro finished. Activating Gameplay Player.");

            // 1. 플레이어 전환 및 위치 동기화
            if (cutscenePlayer != null && gameplayPlayer != null)
            {
                gameplayPlayer.transform.position = cutscenePlayer.transform.position;
                gameplayPlayer.transform.rotation = cutscenePlayer.transform.rotation;
                
                cutscenePlayer.SetActive(false);
                gameplayPlayer.SetActive(true);
            }

            // 2. 카메라 타겟 업데이트
            if (playerCamera != null && gameplayPlayer != null)
            {
                // PlayerCameraRoot 자식을 찾아서 Follow/LookAt 설정
                Transform target = gameplayPlayer.transform.Find("PlayerCameraRoot");
                if (target == null) target = gameplayPlayer.transform;

                playerCamera.Follow = target;
                playerCamera.LookAt = target;
                
                // 카메라 우선순위 상향
                playerCamera.Priority.Value = 10;
            }

            // 3. 게임플레이 플레이어 태그 확인
            if (gameplayPlayer != null && !gameplayPlayer.CompareTag("Player"))
            {
                gameplayPlayer.tag = "Player";
            }
        }
    }
}
