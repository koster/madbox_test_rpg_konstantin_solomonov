using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    public Slider slider;
    public Unit enemy;

    void Update()
    {
        if (enemy == null || enemy.GetState() == UnitState.DEAD)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = enemy.transform.position + Vector3.up * enemy.height;
        slider.value = (float)enemy.GetHealth() / enemy.maxHealth;
    }
}