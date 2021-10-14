using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : GameService
{
    public Unit playerPrefab;
    public CinemachineVirtualCamera cinemachine;

    public List<Weapon> startingWeapons = new List<Weapon>();

    JoystickInput joystick;
    Unit unit;
    int selectedWeapon;

    public override void Init()
    {
        unit = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        cinemachine.Follow = unit.transform;

        joystick = Main.Get<JoystickInput>();

        var range = Random.Range(0, startingWeapons.Count);
        var startingWeapon = startingWeapons[range];
        ChangeWeapon(startingWeapon);
    }

    public Vector3 GetPosition()
    {
        return unit.transform.position;
    }

    public void ChangeWeapon(Weapon to)
    {
        unit.Equip(to);
        Main.Get<GameEvents>().PlayerWeaponChanged?.Invoke(to);
    }

    void Update()
    {
        unit.SetMoveDirection(joystick.GetMoveVector());
    }

    public void NextWeapon()
    {
        selectedWeapon++;
        ChangeWeapon(startingWeapons[selectedWeapon % startingWeapons.Count]);
    }
}