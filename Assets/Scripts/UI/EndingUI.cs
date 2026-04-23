using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    public Button restartButton;

    void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => {
                GameSceneManager.Instance.LoadScene(GameSceneManager.SceneType.Opening);
            });
        }
    }
}
