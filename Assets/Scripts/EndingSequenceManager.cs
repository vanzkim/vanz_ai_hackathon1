using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
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

    private bool triggerHit = false;

    void Start()
    {
        StartCoroutine(ExecuteSequence());
    }

    IEnumerator ExecuteSequence()
    {
        // 1. Start with Cutscene 1
        Debug.Log("Starting Cutscene 1");
        
        // Use CutsceneManager for proper swapping
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.StartCutscene(cutscene1Player);
        }
        else if (player != null)
        {
            player.SetActive(false);
        }

        if (cutscene1UI != null) cutscene1UI.SetActive(true);
        
        // Wait for Cutscene 1 Timeline if director exists
        var director1 = cutscene1UI != null ? cutscene1UI.GetComponent<PlayableDirector>() : null;
        if (director1 != null)
        {
            yield return null; // Wait one frame for state to update
            yield return new WaitUntil(() => director1.state != PlayState.Playing);
        }
        else
        {
            yield return new WaitForSeconds(3f); // Fallback
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

        // 2. Player movement enabled via EndCutscene or fallback
        Debug.Log("Player can now walk to the trigger");

        // Wait for trigger
        while (!triggerHit)
        {
            yield return null;
        }

        // 3. Play Cutscene 2
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

        // Wait for Cutscene 2 Timeline if director exists
        var director2 = cutscene2UI != null ? cutscene2UI.GetComponent<PlayableDirector>() : null;
        if (director2 != null)
        {
            yield return null;
            yield return new WaitUntil(() => director2.state != PlayState.Playing);
        }
        else
        {
            yield return new WaitForSeconds(3f); // Fallback
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
        if (creditsUI != null) creditsUI.SetActive(true);
        if (endingMusic != null) endingMusic.Play();

        yield return new WaitForSeconds(10f); // Credit duration

        // 5. Return to Opening Scene
        Debug.Log("Returning to Opening Scene");
        SceneManager.LoadScene(openingSceneName);
    }

    public void OnTriggerHit()
    {
        triggerHit = true;
    }
}
