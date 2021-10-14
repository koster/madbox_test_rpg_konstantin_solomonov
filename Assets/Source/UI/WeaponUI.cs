using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Transform slot;
    public Button changeButton;

    Weapon shownWeapon;

    void Start()
    {
        changeButton.onClick.AddListener(ChangeWeapon);
        Main.Get<GameEvents>().PlayerWeaponChanged.AddListener(ShowPlayerWeapon);
    }

    void ShowPlayerWeapon(Weapon weapon)
    {
        if (shownWeapon != null)
            Destroy(shownWeapon.gameObject);

        shownWeapon = Instantiate(weapon, slot);
    }

    void ChangeWeapon()
    {
        Main.Get<Player>().NextWeapon();
    }

    void Update()
    {
        if (shownWeapon != null)
            shownWeapon.transform.Rotate(0f, 60f * Time.deltaTime, 0);
    }
}