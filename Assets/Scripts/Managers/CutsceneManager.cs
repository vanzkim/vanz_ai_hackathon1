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
                    _instance = Object.FindAnyObjectByType<CutsceneManager>();
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

        /// <summary>
        /// 현재 컷씬이 상영 중인지 여부 (컷씬 플레이어가 활성화되어 있는지 확인)
        /// </summary>
        public bool IsCutsceneActive => _activeCutscenePlayer != null && _activeCutscenePlayer.activeInHierarchy;

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
            // Unity의 == null 연산자는 파괴된 Object도 null로 평가하므로
            // 이전 씬에서 destroy된 참조는 자연스럽게 null로 잡힌다.
            // 진행 중인 컷씬 상태(_activeCutscenePlayer)나 이미 비활성화된
            // gameplayPlayer를 강제로 덮어쓰지 않는다. 활성/비활성 제어는
            // StartCutscene/EndCutscene에서만 수행한다.

            // 파괴/부재 참조만 새 씬에서 재탐색
            ResolveSceneReferences();
        }

        /// <summary>
        /// 현재 활성 씬에서 플레이어와 카메라를 탐색해 참조를 갱신한다.
        /// </summary>
        private void ResolveSceneReferences()
        {
            var scene = SceneManager.GetActiveScene();
            // scene.isLoaded 체크를 제거하여 Awake/Start 시점에도 탐색 허용
            var roots = scene.GetRootGameObjects();

            if (gameplayPlayer == null)
            {
                foreach (var root in roots)
                {
                    if (root.name == VanzConstants.PlayerModelName)
                    {
                        gameplayPlayer = root;
                        break;
                    }
                    var found = FindChildRecursive(root.transform, VanzConstants.PlayerModelName);
                    if (found != null)
                    {
                        gameplayPlayer = found.gameObject;
                        break;
                    }
                }
            }

            if (mainGameplayCamera == null)
            {
                foreach (var root in roots)
                {
                    var found = FindChildRecursive(root.transform, VanzConstants.GameplayCameraName);
                    if (found != null)
                    {
                        mainGameplayCamera = found.GetComponent<CinemachineCamera>();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 컷씬 시작 시 호출. 게임플레이 모델을 숨기고 컷씬 모델을 활성화합니다.
        /// </summary>
        public void StartCutscene(GameObject cutscenePlayer)
        {
            // 무조건 최신 참조를 다시 확인
            ResolveSceneReferences();
            _activeCutscenePlayer = cutscenePlayer;

            // 1. gameplayPlayer를 명시적으로 비활성화 (Tag 미설정 프리팹에도 안전)
            if (gameplayPlayer != null && gameplayPlayer != _activeCutscenePlayer)
            {
                gameplayPlayer.SetActive(false);
            }

            // 2. 'Player' 태그가 붙은 보조 오브젝트도 비활성화 (중복 플레이어 제거)
            var allPlayers = GameObject.FindGameObjectsWithTag(VanzConstants.PlayerTag);
            foreach (var p in allPlayers)
            {
                if (p != _activeCutscenePlayer)
                {
                    p.SetActive(false);
                }
            }

            // 3. 컷씬 모델 활성화 및 위치 동기화
            if (_activeCutscenePlayer != null)
            {
                if (gameplayPlayer != null)
                {
                    _activeCutscenePlayer.transform.position = gameplayPlayer.transform.position;
                    _activeCutscenePlayer.transform.rotation = gameplayPlayer.transform.rotation;
                }
                _activeCutscenePlayer.SetActive(true);
            }
        }

        /// <summary>
        /// 컷씬 종료 시 호출. 컷씬 모델의 최종 위치를 게임플레이 모델에 복사하고 제어권을 복원합니다.
        /// </summary>
        /// <param name="cutsceneActor">사용되었던 컷씬 전용 모델. null 가능.</param>
        public void EndCutscene(GameObject cutsceneActor = null)
        {
            // 무조건 최신 참조 다시 확인
            ResolveSceneReferences();
            
            var finalActor = cutsceneActor ?? _activeCutscenePlayer;

            if (gameplayPlayer != null)
            {
                if (finalActor != null)
                {
                    // CharacterController가 있는 경우 텔레포트 시 물리 연산 간섭 방지를 위해 일시 비활성화
                    var cc = gameplayPlayer.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    // 위치 및 회전 동기화 (컷씬이 끝난 지점에서 플레이어 재등장)
                    gameplayPlayer.transform.position = finalActor.transform.position;
                    gameplayPlayer.transform.rotation = finalActor.transform.rotation;

                    if (cc != null) cc.enabled = true;

                    finalActor.SetActive(false);
                }

                gameplayPlayer.SetActive(true);
            }

            // --- 카메라 복구 로직 강화 ---
            if (mainGameplayCamera != null)
            {
                // 1. 카메라 오브젝트 활성화
                mainGameplayCamera.gameObject.SetActive(true);

                // 2. 타겟 재설정
                Transform target = null;
                if (gameplayPlayer != null)
                {
                    target = FindChildRecursive(gameplayPlayer.transform, VanzConstants.PlayerCameraRootName);
                    if (target == null) target = gameplayPlayer.transform;
                }

                if (target != null)
                {
                    mainGameplayCamera.Follow = target;
                    mainGameplayCamera.LookAt = target;
                }

                // 3. 우선순위를 높여서 즉시 전환 유도 (타임라인 카메라보다 높게)
                mainGameplayCamera.Priority = 100;

                // 4. CinemachineBrain 활성화 확인
                var brain = Camera.main != null ? Camera.main.GetComponent<CinemachineBrain>() : null;
                if (brain != null) brain.enabled = true;

                Debug.Log($"[CutsceneManager] Camera restored: Priority={mainGameplayCamera.Priority}");
            }
            else
            {
                Debug.LogWarning("[CutsceneManager] Gameplay Camera not found, skipped restoration.");
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
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var deep = FindChildRecursive(parent.GetChild(i), name);
                if (deep != null) return deep;
            }
            return null;
        }
    }
}
