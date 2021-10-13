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

        SceneManager.LoadScene("UI Scene", LoadSceneMode.Additive);
    }
}