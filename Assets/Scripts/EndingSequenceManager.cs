using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using System.Collections;
using VanzAI.Managers;

public class EndingSequenceManager : MonoBehaviour
{
    [Header("Cutscenes (Directors)")]
    public GameObject cutscene1UI;
    public GameObject cutscene2UI;

    [Header("Cutscene Actors")]
    public GameObject cutscene1Player;
    public GameObject cutscene2Player;

    [Header("Sequence Settings")]
    public GameObject player;
    public GameObject creditsUI;
    public AudioSource endingMusic;
    public string openingSceneName = "OpeningScene";

    [Header("Credits Settings")]
    public float creditFadeDuration = 2.0f;
    public float creditDisplayDuration = 10.0f;

    private bool triggerHit = false;

    void Start()
    {
        StartCoroutine(ExecuteSequence());
    }

    IEnumerator ExecuteSequence()
    {
        // ... (Cutscene 1 and 2 logic remains same)
        // 1. Start with Cutscene 1
        Debug.Log("Starting Cutscene 1");
        
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.StartCutscene(cutscene1Player);
        }
        else if (player != null)
        {
            player.SetActive(false);
        }

        if (cutscene1UI != null) cutscene1UI.SetActive(true);
        
        var director1 = cutscene1UI != null ? cutscene1UI.GetComponent<PlayableDirector>() : null;
        if (director1 != null)
        {
            yield return null; 
            yield return new WaitUntil(() => director1.state != PlayState.Playing);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }
        
        if (cutscene1UI != null) cutscene1UI.SetActive(false);
        
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.EndCutscene(cutscene1Player);
        }
        else if (player != null)
        {
            player.SetActive(true);
        }
        
        Debug.Log("Cutscene 1 Finished");

        Debug.Log("Player can now walk to the trigger");

        while (!triggerHit)
        {
            yield return null;
        }

        Debug.Log("Starting Cutscene 2");
        
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.StartCutscene(cutscene2Player);
        }
        else if (player != null)
        {
            player.SetActive(false);
        }

        if (cutscene2UI != null) cutscene2UI.SetActive(true);

        var director2 = cutscene2UI != null ? cutscene2UI.GetComponent<PlayableDirector>() : null;
        if (director2 != null)
        {
            yield return null;
            yield return new WaitUntil(() => director2.state != PlayState.Playing);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        if (cutscene2UI != null) cutscene2UI.SetActive(false);
        
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.EndCutscene(cutscene2Player);
        }
        else if (player != null)
        {
            player.SetActive(true);
        }

        Debug.Log("Cutscene 2 Finished");

        // 4. Show Credits and Play Music
        Debug.Log("Showing Credits and playing music");
        if (creditsUI != null)
        {
            creditsUI.SetActive(true);
            CanvasGroup cg = creditsUI.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                // Fade in credits
                float elapsed = 0;
                while (elapsed < creditFadeDuration)
                {
                    elapsed += Time.deltaTime;
                    cg.alpha = Mathf.Clamp01(elapsed / creditFadeDuration);
                    yield return null;
                }
                cg.alpha = 1f;
            }
        }
        
        if (endingMusic != null) endingMusic.Play();

        // Wait for time or input
        float displayTimer = 0;
        bool skipRequested = false;

        while (displayTimer < creditDisplayDuration && !skipRequested)
        {
            displayTimer += Time.deltaTime;

            // Check for any key or mouse click
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) skipRequested = true;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) skipRequested = true;

            yield return null;
        }

        // 5. Return to Opening Scene
        Debug.Log("Returning to Opening Scene");
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadScene(GameSceneManager.SceneType.Opening);
        }
        else
        {
            SceneManager.LoadScene(openingSceneName);
        }
    }

    public void OnTriggerHit()
    {
        triggerHit = true;
    }
}
