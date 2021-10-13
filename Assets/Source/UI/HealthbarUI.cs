using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    public Slider slider;
    public Enemy enemy;

    void Update()
    {
        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = enemy.transform.position + Vector3.up * enemy.height;
        slider.value = (float)enemy.GetHealth() / enemy.maxHealth;
    }
}