using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Button tryAgain;

    void Start()
    {
        tryAgain.onClick.AddListener(OnClickTryAgain);
    }

    void OnClickTryAgain()
    {
        SceneManager.LoadScene("Combat Scene");
    }
}