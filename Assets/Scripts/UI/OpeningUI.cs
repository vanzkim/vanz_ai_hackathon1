using UnityEngine;
using UnityEngine.UI;

public class OpeningUI : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => {
                GameSceneManager.Instance.LoadScene(GameSceneManager.SceneType.Hospital);
            });
        }
    }
}
