using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingSequenceManager : MonoBehaviour
{
    [Header("Cutscenes (Dummy)")]
    public GameObject cutscene1UI;
    public GameObject cutscene2UI;

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
        if (player != null) player.SetActive(false);
        if (cutscene1UI != null) cutscene1UI.SetActive(true);
        
        yield return new WaitForSeconds(3f); // Dummy duration
        
        if (cutscene1UI != null) cutscene1UI.SetActive(false);
        Debug.Log("Cutscene 1 Finished");

        // 2. Enable Player movement
        if (player != null) player.SetActive(true);
        Debug.Log("Player can now walk to the trigger");

        // Wait for trigger
        while (!triggerHit)
        {
            yield return null;
        }

        // 3. Play Cutscene 2
        Debug.Log("Starting Cutscene 2");
        if (player != null) player.SetActive(false);
        if (cutscene2UI != null) cutscene2UI.SetActive(true);

        yield return new WaitForSeconds(3f); // Dummy duration

        if (cutscene2UI != null) cutscene2UI.SetActive(false);
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
