using UnityEngine;
using Unity.Cinemachine;

namespace VanzAI.Managers
{
    /// <summary>
    /// 게임플레이 모델과 컷씬 모델 간의 전환 및 위치 동기화를 관리하는 시스템.
    /// </summary>
    public class CutsceneManager : MonoBehaviour
    {
        private static CutsceneManager _instance;
        public static CutsceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindFirstObjectByType<CutsceneManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("CutsceneManager");
                        _instance = go.AddComponent<CutsceneManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Settings")]
        [SerializeField] private GameObject gameplayPlayer;
        [SerializeField] private CinemachineCamera mainGameplayCamera;

        private GameObject _activeCutscenePlayer;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // 초기 플레이어 및 카메라 자동 탐색
            if (gameplayPlayer == null) gameplayPlayer = GameObject.Find("Player_Model");
            if (mainGameplayCamera == null)
            {
                var camObj = GameObject.Find("CM ThirdPerson Camera");
                if (camObj != null) mainGameplayCamera = camObj.GetComponent<CinemachineCamera>();
            }
        }

        /// <summary>
        /// 컷씬 시작 시 호출. 게임플레이 모델을 숨기고 컷씬 모델을 활성화합니다.
        /// </summary>
        public void StartCutscene(GameObject cutscenePlayer)
        {
            _activeCutscenePlayer = cutscenePlayer;

            if (gameplayPlayer != null)
            {
                // 위치 동기화 (컷씬 모델이 시작할 위치로 게임플레이 모델이 먼저 이동해 있으면 자연스러움)
                if (_activeCutscenePlayer != null)
                {
                    _activeCutscenePlayer.transform.position = gameplayPlayer.transform.position;
                    _activeCutscenePlayer.transform.rotation = gameplayPlayer.transform.rotation;
                    _activeCutscenePlayer.SetActive(true);
                }
                
                gameplayPlayer.SetActive(false);
            }
        }

        /// <summary>
        /// 컷씬 종료 시 호출. 컷씬 모델의 최종 위치를 게임플레이 모델에 복사하고 제어권을 복원합니다.
        /// </summary>
        public void EndCutscene()
        {
            if (gameplayPlayer == null) return;

            if (_activeCutscenePlayer != null)
            {
                // 위치 및 회전 동기화 (컷씬이 끝난 지점에서 플레이어 재등장)
                gameplayPlayer.transform.position = _activeCutscenePlayer.transform.position;
                gameplayPlayer.transform.rotation = _activeCutscenePlayer.transform.rotation;
                
                _activeCutscenePlayer.SetActive(false);
            }

            gameplayPlayer.SetActive(true);

            // 카메라 타겟 다시 연결
            if (mainGameplayCamera != null)
            {
                Transform target = gameplayPlayer.transform.Find("PlayerCameraRoot");
                if (target == null) target = gameplayPlayer.transform;
                
                mainGameplayCamera.Follow = target;
                mainGameplayCamera.LookAt = target;
            }

            _activeCutscenePlayer = null;
            Debug.Log("[CutsceneManager] Cutscene finalized. Control returned to Gameplay Player.");
        }
    }
}
