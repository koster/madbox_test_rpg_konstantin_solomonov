using TMPro;
using UnityEngine;

public class FloatingTextUI : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float lifetime = 1f;
    public float floatSpeed = 1f;

    float clock;
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ResetFromPool()
    {
        clock = 0f;
    }

    void Update()
    {
        clock += Time.deltaTime;
        canvasGroup.alpha = (lifetime - clock) / lifetime;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}