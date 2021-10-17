using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnClickStart);
    }

    void OnClickStart()
    {
        SceneManager.LoadScene("Combat Scene");
    }
}