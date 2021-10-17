using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static Main i;

    public static T Get<T>() where T : Component
    {
        return i.GetComponent<T>();
    }

    void Awake()
    {
        i = this;
        Application.targetFrameRate = 60;
        SceneManager.LoadScene("UI Scene", LoadSceneMode.Additive);
    }

    void Start()
    {
        var services = GetComponents<GameService>();

        foreach (var service in services)
            service.Init();
        
        foreach (var service in services)
            service.GameStarted();
    }
}