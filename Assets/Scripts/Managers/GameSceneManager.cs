using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager _instance;
    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameSceneManager");
                _instance = obj.AddComponent<GameSceneManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    [Header("Transition Settings")]
    public float fadeDuration = 1.0f;
    private Canvas _fadeCanvas;
    private Image _fadeImage;

    public enum SceneType
    {
        Opening,
        Hospital,
        Ending
    }

    private string GetSceneName(SceneType type)
    {
        return type switch
        {
            SceneType.Opening => "OpeningScene",
            SceneType.Hospital => "Hespital_EXAMPLE_copy",
            SceneType.Ending => "EndingScene",
            _ => "OpeningScene"
        };
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        SetupFadeUI();
    }

    private void SetupFadeUI()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        _fadeCanvas = canvasObj.AddComponent<Canvas>();
        _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _fadeCanvas.sortingOrder = 999;
        
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        _fadeImage = imageObj.AddComponent<Image>();
        _fadeImage.color = new Color(0, 0, 0, 0);
        
        RectTransform rt = _fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        
        _fadeImage.raycastTarget = false;
    }

    public void LoadScene(SceneType sceneType)
    {
        StartCoroutine(TransitionToScene(GetSceneName(sceneType)));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        _fadeImage.raycastTarget = true;
        yield return Fade(1f);
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return Fade(0f);
        _fadeImage.raycastTarget = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = _fadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        _fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}
