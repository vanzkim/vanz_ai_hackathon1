using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

namespace VanzAI.Managers
{
    /// <summary>
    /// 게임플레이 모델과 컷씬 모델 간의 전환 및 위치 동기화를 관리하는 시스템.
    /// DontDestroyOnLoad 싱글톤이므로 씬 전환마다 참조를 재탐색한다.
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

            // 최초 씬 참조 확보
            ResolveSceneReferences();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 씬 전환 시 이전 씬의 파괴된 오브젝트 참조를 끊고 재탐색한다.
            _activeCutscenePlayer = null;
            gameplayPlayer = null;
            mainGameplayCamera = null;
            ResolveSceneReferences();
        }

        /// <summary>
        /// 현재 활성 씬에서 플레이어와 카메라를 탐색해 참조를 갱신한다.
        /// </summary>
        private void ResolveSceneReferences()
        {
            if (gameplayPlayer == null)
            {
                gameplayPlayer = GameObject.Find(VanzConstants.PlayerModelName);
            }

            if (mainGameplayCamera == null)
            {
                var camObj = GameObject.Find(VanzConstants.GameplayCameraName);
                if (camObj != null) mainGameplayCamera = camObj.GetComponent<CinemachineCamera>();
            }
        }

        /// <summary>
        /// 컷씬 시작 시 호출. 게임플레이 모델을 숨기고 컷씬 모델을 활성화합니다.
        /// </summary>
        public void StartCutscene(GameObject cutscenePlayer)
        {
            // Lazy-resolve: 씬 로드 콜백이 늦게 와도 여기서 안전망 확보
            if (gameplayPlayer == null) ResolveSceneReferences();

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
            else if (_activeCutscenePlayer != null)
            {
                // gameplayPlayer 부재 시에도 컷씬 모델은 활성화(예: Opening 직후 인트로)
                _activeCutscenePlayer.SetActive(true);
            }
        }

        /// <summary>
        /// 컷씬 종료 시 호출. 컷씬 모델의 최종 위치를 게임플레이 모델에 복사하고 제어권을 복원합니다.
        /// </summary>
        public void EndCutscene()
        {
            if (gameplayPlayer == null) ResolveSceneReferences();
            if (gameplayPlayer == null)
            {
                // 여전히 없으면 컷씬 모델만 정리하고 종료
                if (_activeCutscenePlayer != null)
                {
                    _activeCutscenePlayer.SetActive(false);
                    _activeCutscenePlayer = null;
                }
                Debug.LogWarning("[CutsceneManager] EndCutscene: gameplayPlayer not found in current scene.");
                return;
            }

            if (_activeCutscenePlayer != null)
            {
                // 위치 및 회전 동기화 (컷씬이 끝난 지점에서 플레이어 재등장)
                gameplayPlayer.transform.position = _activeCutscenePlayer.transform.position;
                gameplayPlayer.transform.rotation = _activeCutscenePlayer.transform.rotation;

                _activeCutscenePlayer.SetActive(false);
            }

            gameplayPlayer.SetActive(true);

            // 카메라 타겟 다시 연결 (중첩 구조 대응: 재귀 탐색)
            if (mainGameplayCamera != null)
            {
                Transform target = FindChildRecursive(gameplayPlayer.transform, VanzConstants.PlayerCameraRootName);
                if (target == null) target = gameplayPlayer.transform;

                mainGameplayCamera.Follow = target;
                mainGameplayCamera.LookAt = target;
            }

            _activeCutscenePlayer = null;
            Debug.Log("[CutsceneManager] Cutscene finalized. Control returned to Gameplay Player.");
        }

        /// <summary>
        /// 자식 트리를 재귀적으로 탐색해 이름이 일치하는 Transform을 반환.
        /// </summary>
        private static Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent == null) return null;
            for (int i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i);
                if (c.name == name) return c;
                var deep = FindChildRecursive(c, name);
                if (deep != null) return deep;
            }
            return null;
        }
    }
}
