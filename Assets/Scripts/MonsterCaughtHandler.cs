using UnityEngine;
using UnityEngine.Playables;
using VanzAI.Managers;

/// <summary>
/// 몬스터가 플레이어와 충돌했을 때 컷씬을 재생하고 플레이어를 특정 위치로 이동시키는 스크립트.
/// </summary>
public class MonsterCaughtHandler : MonoBehaviour
{
    [Header("Cutscene Settings")]
    [Tooltip("재생할 컷씬 타임라인")]
    [SerializeField] private PlayableDirector cutsceneDirector;
    
    [Tooltip("컷씬 동안 사용할 전용 모델 (있을 경우)")]
    [SerializeField] private GameObject cutscenePlayer;

    [Header("Restart Settings")]
    [Tooltip("컷씬 종료 후 플레이어가 이동할 위치")]
    [SerializeField] private Transform restartPoint;

    private bool isCaught = false;
    private Vector3 monsterStartPos;
    private Quaternion monsterStartRot;

    private void Awake()
    {
        monsterStartPos = transform.position;
        monsterStartRot = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCaught) return;

        // 충돌한 대상이 플레이어인지 확인
        if (IsPlayer(other))
        {
            isCaught = true;
            StartCutscene();
        }
    }

    private bool IsPlayer(Collider other)
    {
        // VanzConstants.PlayerModelName ("Player_Model") 또는 태그로 확인
        var root = other.transform.root;
        bool isPlayer = root.name == "Player_Model" || other.name == "Player_Model" || other.CompareTag("Player");
        return isPlayer;
    }

    private void StartCutscene()
    {
        Debug.Log("[MonsterCaughtHandler] Player caught! Starting cutscene.");

        // 몬스터 추격 중지
        var monsterChase = GetComponent<MonsterChase>();
        if (monsterChase != null) monsterChase.enabled = false;

        if (cutsceneDirector != null)
        {
            // 컷씬 오브젝트를 현재 충돌 위치로 이동 (그 자리에서 보여지게 함)
            cutsceneDirector.transform.position = transform.position;
            cutsceneDirector.transform.rotation = transform.rotation;

            // CutsceneManager를 사용하여 모델 스왑 및 위치 동기화 처리
            CutsceneManager.Instance.StartCutscene(cutscenePlayer);

            // 컷씬이 끝날 때 RestartPlayer 호출
            cutsceneDirector.stopped += OnCutsceneStopped;
            cutsceneDirector.Play();
        }
        else
        {
            Debug.LogWarning("[MonsterCaughtHandler] No cutscene director assigned. Restarting immediately.");
            RestartPlayer();
        }
    }

    private void OnCutsceneStopped(PlayableDirector director)
    {
        director.stopped -= OnCutsceneStopped;
        
        // 모델 스왑 복구 및 제어권 반환
        CutsceneManager.Instance.EndCutscene(cutscenePlayer);
        
        RestartPlayer();
    }

    private void RestartPlayer()
    {
        Debug.Log("[MonsterCaughtHandler] Restarting player and monster positions.");

        // 몬스터 추격 다시 활성화 및 초기 위치로 복귀 시작
        var monsterChase = GetComponent<MonsterChase>();
        if (monsterChase != null)
        {
            monsterChase.enabled = true;
            monsterChase.ResetMonster();
        }

        GameObject player = GameObject.Find("Player_Model");
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && restartPoint != null)
        {
            // CharacterController 비활성화 후 이동 (텔레포트 간섭 방지)
            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = restartPoint.position;
            player.transform.rotation = restartPoint.rotation;

            if (cc != null) cc.enabled = true;
        }

        isCaught = false;

        // --- Monster_GreyLady 예외 처리 ---
        if (gameObject.name == "Monster_GreyLady")
        {
            Debug.Log("[MonsterCaughtHandler] GreyLady caught player. Disabling GreyLady and Meet_white cutscene.");
            
            // Meet_white 컷씬 비활성화
            GameObject meetWhite = GameObject.Find("Meet_white");
            if (meetWhite != null)
            {
                meetWhite.SetActive(false);
            }
            
            // 자기 자신(Monster_GreyLady) 비활성화
            gameObject.SetActive(false);
        }
        }
        }
