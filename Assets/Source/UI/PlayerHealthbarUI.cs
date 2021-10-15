using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbarUI : MonoBehaviour
{
    public Slider slider;

    void Update()
    {
        var unit = Main.Get<Player>().unit;
        slider.value = (float)unit.health / unit.data.maxHealth;

        if (unit.IsDead())
            Destroy(gameObject);
    }
}