using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    public Slider slider;
    public Unit enemy;

    void Update()
    {
        if (enemy == null || enemy.IsDead())
        {
            Destroy(gameObject);
            return;
        }

        transform.position = enemy.transform.position + Vector3.up * enemy.data.height;
        slider.value = (float)enemy.health / enemy.data.maxHealth;
    }
}